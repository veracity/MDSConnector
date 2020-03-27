using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Certificate;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using MDSConnector.Utilities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using MDSConnector.Utilities.ConfigHelpers;
using MDSConnector.APIClients;
using System.Text;
using MDSConnector.Authentication;
using MDSConnector.Utilities.Time;

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
            IConfigurationSection mdsSection = Configuration.GetSection("MDSConfig");
            services.Configure<MDSConfig>(mdsSection);
            IConfigurationSection azureStorageSection = Configuration.GetSection("AzureStorageConfig");
            services.Configure<AzureStorageConfig>(azureStorageSection);
            IConfiguration knownCertificateIssuersSection = Configuration.GetSection("KnownCertificateIssuers");
            services.Configure<KnownCertificateIssuers>(knownCertificateIssuersSection);

            services.AddScoped<HttpClient>();
            services.AddScoped<IMDSClient, MDSClient>();
            services.AddScoped<IAzureStorageClient, AzureStorageClient>();
            services.AddSingleton<ITimeProvider, DefaultTimeProvider>();

            services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();
            });

            services.AddDistributedMemoryCache();


            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "CustomCertificationAuthentication";
            })
            .AddScheme<AuthenticationSchemeOptions, CustomCertificateAuthenticator>("CustomCertificationAuthentication", null);
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
