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
            services.AddTransient<ICertificateVerifier, DemoCertificateVerifier>();
            services.AddAuthentication(
                CertificateAuthenticationDefaults.AuthenticationScheme)
                .AddCertificate(options =>
                {
                    options.AllowedCertificateTypes = CertificateTypes.All;
                    options.ValidateCertificateUse = false;
                    options.Events = new CertificateAuthenticationEvents
                    {
                        OnCertificateValidated = context =>
                        {
                            var certificate = context.ClientCertificate;

                            if (certificate == null)
                            {
                                context.Fail("You have not correctly attached a X509 certificate with your request");
                                return Task.CompletedTask;
                            }


                            var validationService = context.HttpContext.RequestServices.GetService<ICertificateVerifier>();

                            var validationResult = validationService.verify(certificate);
                            //Console.WriteLine(validationResult);
                            if (validationResult.valid)
                            {
                                context.Success();
                            }
                            else
                            {
                                context.Fail(validationResult.reason);
                            }

                            return Task.CompletedTask;
                        }
                    };


                });
            services.AddControllers();

            services.AddCertificateForwarding(
                options =>
                {
                    options.CertificateHeader = "X-ARR-ClientCert";
                    options.HeaderConverter = headerValue =>
                    {
                        X509Certificate2 certificate = null;
                        if (!string.IsNullOrWhiteSpace(headerValue))
                        {
                            Console.WriteLine(headerValue);
                            //byte[] certAsBytes = Convert.ToByte(headerValue);
                            byte[] certAsBytes = hexStringToBytes(headerValue);
                            certificate = new X509Certificate2(certAsBytes);
                        }


                        return certificate;
                    };
                });

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

            app.UseCertificateForwarding();
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void testFunction(X509Certificate2 certificate)
        {
            Debug.WriteLine("\n\n " + certificate.ToString() + "\n\n");
        }


        public byte[] hexStringToBytes(string hexValue)
        {
            string cleaned = hexValue.Replace(" ", "");
            byte[] bytes = new byte[cleaned.Length / 2];
            for (int i = 0; i < cleaned.Length; i+=2)
            {
                bytes[i / 2] = Convert.ToByte(hexValue.Substring(i, 2), 16);
            }
            return bytes;

        }

    }
}
