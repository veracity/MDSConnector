using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using MDSConnector.Utilities;
using System.Net.Http;
using MDSConnector.Utilities.ConfigHelpers;
using MDSConnector.APIClients;
using MDSConnector.Utilities.Time;
using MDSConnector.Utilities.EUMRV;
using Azure.Security.KeyVault.Certificates;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace MDSConnector
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Initiate config objects
            string NeuronUrl = Configuration.GetValue<string>("NeuronUrl");
            string sasToken = Configuration.GetValue<string>("sasToken");

            var mdsConfig = new MDSConfig();
            mdsConfig.NeuronUrl = NeuronUrl;
            services.AddSingleton(mdsConfig);

            var azureStorageConfig = new AzureStorageConfig();
            azureStorageConfig.sasToken = sasToken;
            services.AddSingleton(azureStorageConfig);
            //Add objects that are needed for different parts of the program to the serviceCollection

            var keyvaultName = Configuration.GetValue<string>("KeyVaultUri");
            var certificateName = Configuration.GetValue<string>("CertificateName");

            var httpClient = InitiateHttpClientWithCertificate(keyvaultName, certificateName);
            services.AddSingleton(httpClient);
            services.AddSingleton<IMDSClient, MDSClient>();
            services.AddScoped<IAzureStorageClient, AzureStorageClient>();
            services.AddSingleton<ITimeProvider, DefaultTimeProvider>();
            services.AddSingleton<IEUMRVReportGenerator, BasicEUMRVReportGenerator>();

            services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();
            });


            services.AddControllers();

            ////Configure authentication, this is where the custom authentication scheme/method is specified.
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = "CustomCertificationAuthentication";
            //})
            //.AddScheme<AuthenticationSchemeOptions, CustomCertificateAuthenticationHandler>("CustomCertificationAuthentication", null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //specify that authentication is used
            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private HttpClient InitiateHttpClientWithCertificate(string keyvaultName, string certificateName)
        {

            var certificateClient = new CertificateClient(new Uri(keyvaultName), new DefaultAzureCredential());
            var secretClient = new SecretClient(new Uri(keyvaultName), new DefaultAzureCredential());
            var keyvaultCertificateWithPolicy = certificateClient.GetCertificate(certificateName);

            Uri secretId = keyvaultCertificateWithPolicy.Value.SecretId;
            var segments = secretId.Segments;
            string secretName = segments[2].Trim('/');
            string version = segments[3].TrimEnd('/');

            KeyVaultSecret secret = secretClient.GetSecret(secretName, version);
            
            byte[] privateKeyBytes = Convert.FromBase64String(secret.Value);
            var certificate = new X509Certificate2(privateKeyBytes, "");

            var clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            };
            clientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
            clientHandler.ClientCertificates.Add(certificate);
            
            return new HttpClient(clientHandler);
        }
    }
}
