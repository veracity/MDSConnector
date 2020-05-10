﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Authentication;
using System.Net.Security;

namespace MDSClient
{

    /// <summary>
    /// An simple client program that can communicate with the MDSConnector, authenticated using X509Certificate.
    /// Can be used to demonstrate the certificate authentication process.
    /// </summary>
    class Program
    {

        async static Task Main(string[] args)
        {
            Console.WriteLine("This is the Maritime data space client.");
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.WriteLine("Loading certificate");

            var certificatePaths = new Dictionary<string, string>
            {
                {"admin", @"\clientCertificates\root_ca_dnvgl_dev.pfx"},
                {"crt", @"\clientCertificates\root_ca_dnvgl_dev.crt"},
                {"expired", @"\clientCertificates\dnvgl_expired.pfx"},
                {"microsoft", @"\clientCertificates\root_ca_microsoft_dev.pfx"},
                {"google", @"\clientCertificates\root_ca_google.pfx"}
            };


            var certificateFromFile = await loadCertificate(Directory.GetCurrentDirectory() + certificatePaths["expired"], "1234");
            HttpClientSingleton.create(certificateFromFile);

            var headers = new Dictionary<string, string>();
            var request = buildRequest("https://localhost:10001/",
                        HttpMethod.Get,
                        headers);
            var response = await HttpClientSingleton.Instance.sendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();


            headers = new Dictionary<string, string>();
            var adminRequest = buildRequest("https://localhost:10001/admin",
                                    HttpMethod.Get,
                                    headers);
            var adminResponse = await HttpClientSingleton.Instance.sendAsync(adminRequest);
            var adminResponseString = await adminResponse.Content.ReadAsStringAsync();

            Console.WriteLine("\nClaims endpoint");
            Console.WriteLine($"Response code: {response.StatusCode}");
            Console.WriteLine($"Claims: {responseString}");
            Console.WriteLine("\nAdmin endpoint");
            Console.WriteLine($"Res code: {adminResponse.StatusCode}");
            Console.WriteLine($"Resposne: {adminResponseString}");
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
                httpRequestMessage.Headers.Add(item.Key, item.Value);
            }

            return httpRequestMessage;
        }

        private async static Task<X509Certificate2> loadCertificate(string path, string password)
        {
            X509Certificate2 certificate = null;
            if (path.Contains(".pfx"))
            {
                certificate = new X509Certificate2(path, password);
            }
            else
            {
                var certAsBytes = await File.ReadAllBytesAsync(path);
                certificate = new X509Certificate2(certAsBytes);

            }
            return certificate;
        }

    }
}
