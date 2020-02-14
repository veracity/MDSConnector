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
            IConfigurationSection veracitySection = Configuration.GetSection("veracityConfig");
            services.Configure<VeracityConfig>(veracitySection);



            //services.BuildServiceProvider()

            

            services.AddScoped<HttpClient>();
            //services.AddSingleton(s => new MDSClient(
            //                                s.GetService<ILogger<MDSClient>>(), 
            //                                s.GetService<HttpClient>(), 
            //                                s.GetService<MDSConfig>()));
            //services.AddSingleton(s => new VeracityClient(
            //                                s.GetService<ILogger<VeracityClient>>(),
            //                                s.GetService<HttpClient>(),
            //                                s.GetService<VeracityConfig>()));
            services.AddSingleton<IMDSClient, MDSClient>();
            services.AddSingleton<IVeracityClient, VeracityClient>();


            services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();
            });

            services.AddControllers();
            services.AddAuthentication(options =>
            {
                //options.DefaultAuthenticateScheme = "CustomCertificationAuthentication";
                options.DefaultScheme = "CustomCertificationAuthentication";
            })
            .AddCertificate()
            .AddScheme<AuthenticationSchemeOptions, CustomCertificateAuthenticator>("CustomCertificationAuthentication", null);

            services.AddTransient<ICertificateVerifier, DemoCertificateVerifier>();

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

            //app.UseCertificateForwarding();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
