using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace MDSClient
{
    class Program
    {

        async static Task Main(string[] args)
        {
            Console.WriteLine("This is the Maritime data space client.");
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.WriteLine("Loading certificate");

            var certificate = await loadCertificate(Directory.GetCurrentDirectory() + @"\clientCertificates\certs\client.pem");
            //var corrupted = File.ReadAllText("clientcertificates\\certs\\corrupted.pem").Replace("\n", "").Replace("\r", "");

            HttpClientSingleton.create(certificate);

            var headers = new Dictionary<string, string>();
            //var request = buildRequest("https://localhost:44365/",
            //                        HttpMethod.Get,
            //                        headers);
            var request = buildRequest("https://localhost:5001",
                                    HttpMethod.Get,
                                    headers);


            var response = await HttpClientSingleton.Instance.sendAsync(request);

            //var handler = new HttpClientHandler();
            //handler.ClientCertificates.Add(certificate);
            //handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            //handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            //var client = new HttpClient(handler);
            //var response = await client.SendAsync(request);



            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Resposne: {responseString}");
            

        }


        private static HttpRequestMessage buildRequest(string uri, HttpMethod method, Dictionary<string, string> headers)
        {
            var httpRequestMessage = new HttpRequestMessage() { 
                Method = method,
                RequestUri = new Uri(uri),
                Headers = {}
            };

            foreach (var item in headers)
            {
                //Console.WriteLine($"Key: {item.Key}, value: {item.Value}");
                httpRequestMessage.Headers.Add(item.Key, item.Value);
            }

            return httpRequestMessage;
        }

        private async static Task<X509Certificate2> loadCertificate(string path)
        {
            var certAsBytes = await File.ReadAllBytesAsync(path);
            //certAsString = certAsString.Replace("\n", "").Replace("\r", "");
            var certificate = new X509Certificate2(certAsBytes);
            Console.WriteLine(certificate.ToString());
            return certificate;
        }

        private static string exportCertificateAsString(X509Certificate2 certificate)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            
            byte[] certificateContents = certificate.Export(X509ContentType.Cert);
            var certificateContentAsString = Convert.ToBase64String(certificateContents, Base64FormattingOptions.InsertLineBreaks);
            builder.AppendLine(certificateContentAsString);

            builder.AppendLine("-----END CERTIFICATE-----");
            
            return builder.ToString();

        }


    }
}
