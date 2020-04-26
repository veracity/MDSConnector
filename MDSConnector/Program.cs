using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace MDSConnector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(o =>
                    {
                        //Configure webserver to accept certificates. 
                        //By using the following settings, the framework will pass the certificate untouched to the authentication handler that we have defined.
                        //Using other settings, depending on the setting used, the server will reject requests that does not meet with the ASP.Net buildt in criterias.
                        //See documentation: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth?view=aspnetcore-3.1
                        o.ConfigureHttpsDefaults(o =>
                        {
                            o.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                            o.CheckCertificateRevocation = false;
                            o.AllowAnyClientCertificate();
                        });
                        
                    });
                    webBuilder.UseStartup<Startup>();
                });
            
    }
}
