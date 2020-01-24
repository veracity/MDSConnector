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
            services.AddControllers();
            services.AddAuthentication(
                CertificateAuthenticationDefaults.AuthenticationScheme)
                .AddCertificate(options =>
                {
                    options.AllowedCertificateTypes = CertificateTypes.All;
                    options.ValidateCertificateUse = true;
                    options.Events = new CertificateAuthenticationEvents
                    {
                        //OnAuthenticationFailed = context =>
                        //{
                        //    context.Fail("Certification validation failed");
                        //    var certificate = context.HttpContext.Connection.ClientCertificate;
                        //    return Task.CompletedTask;
                        //},
                        OnCertificateValidated = context =>
                        {
                            var certificate = context.ClientCertificate;
                            testFunction(certificate);
                            return Task.CompletedTask;
                        }
                    };


                });
            //services.AddCertificateForwarding(
            //    options => {
            //        options.CertificateHeader
            //    });

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

            app.UseAuthorization();
            app.UseAuthentication();
            //app.UseCertificateForwarding();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void testFunction(X509Certificate2 certificate)
        {
            Debug.WriteLine("\n\n " + certificate.ToString() + "\n\n");
        }


    }
}
