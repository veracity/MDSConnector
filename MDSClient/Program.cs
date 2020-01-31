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

            var certificatePaths = new Dictionary<string, string>
            {
                {"pfx", @"\clientCertificates\root_ca_dnvgl_dev.pfx"},
                {"crt", @"\clientCertificates\root_ca_dnvgl_dev.crt"}
            };

            //var corrupted = File.ReadAllText("clientcertificates\\certs\\corrupted.pem").Replace("\n", "").Replace("\r", "");
            var certificate = await loadCertificate(Path.Combine(Directory.GetCurrentDirectory(), certificatePaths["PFX"]));

            HttpClientSingleton.create(certificate);

            var headers = new Dictionary<string, string>();
            //headers.Add("X-ARR-ClientCert", certificate.GetRawCertDataString());

            var request = buildRequest("https://localhost:10000",
                                    HttpMethod.Get,
                                    headers);


            var response = await HttpClientSingleton.Instance.sendAsync(request);


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
            Console.WriteLine(path);
            var certAsBytes = await File.ReadAllBytesAsync(path);
            X509Certificate2 certificate = null;
            if (path.Contains(".pfx"))
            {
                certificate = new X509Certificate2(path, "1234");
            }
            else
            {
                certificate = new X509Certificate2(certAsBytes);

            }
            Console.WriteLine(certificate.ToString());
            return certificate;
        }

    }
}
