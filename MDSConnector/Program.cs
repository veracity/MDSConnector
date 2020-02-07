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
                        o.ConfigureHttpsDefaults(o =>
                        {
                            //o.SslProtocols = System.Security.Authentication.SslProtocols.Tls11 
                            //                | System.Security.Authentication.SslProtocols.Tls12
                            //                | System.Security.Authentication.SslProtocols.Tls13;
                            //o.SslProtocols = System.Security.Authentication.SslProtocols.None;
                            //o.ServerCertificate = new X509Certificate2("root_ca_dnvgl_dev.pfx", "1234");
                            o.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
