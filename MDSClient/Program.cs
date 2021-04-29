using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Authentication;
using System.Net.Security;
using System.Diagnostics.Tracing;

namespace MDSClient
{

    /// <summary>
    /// An simple client program that can communicate with the MDSConnector, authenticated using X509Certificate.
    /// Can be used to demonstrate the certificate authentication process.
    /// </summary>
    class Program
    {
        //sealed class EventSourceListener : EventListener
        //{
        //    private readonly string _eventSourceName;
        //    private readonly StringBuilder _messageBuilder = new StringBuilder();

        //    public EventSourceListener(string name)
        //    {
        //        _eventSourceName = name;
        //    }

        //    protected override void OnEventSourceCreated(EventSource eventSource)
        //    {
        //        base.OnEventSourceCreated(eventSource);


        //        EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All);

        //    }

        //    protected override void OnEventWritten(EventWrittenEventArgs eventData)
        //    {
        //        base.OnEventWritten(eventData);

        //        string message;
        //        lock (_messageBuilder)
        //        {
        //            _messageBuilder.Append("<- Event ");
        //            _messageBuilder.Append(eventData.EventSource.Name);
        //            _messageBuilder.Append(" - ");
        //            _messageBuilder.Append(eventData.EventName);
        //            _messageBuilder.Append(" : ");
        //            _messageBuilder.AppendJoin(',', eventData.Payload);
        //            _messageBuilder.AppendLine(" ->");
        //            message = _messageBuilder.ToString();
        //            _messageBuilder.Clear();
        //        }
        //        Console.WriteLine(message);
        //    }
        //}
        async static Task Main(string[] args)
        {
            Console.WriteLine("This is the Maritime data space client.");
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.WriteLine(Environment.Version.ToString());

            var certificatePaths = new Dictionary<string, string>
            {
                {"admin", @"\clientCertificates\root_ca_dnvgl_dev.pfx"},
                {"crt", @"\clientCertificates\root_ca_dnvgl_dev.crt"},
                {"expired", @"\clientCertificates\dnvgl_expired.pfx"},
                {"microsoft", @"\clientCertificates\root_ca_microsoft_dev.pfx"},
                {"google", @"\clientCertificates\root_ca_google.pfx"}
            };

            var path = Directory.GetCurrentDirectory() + @"/clientCertificates/dnv_client.pfx";
            //var path = Directory.GetCurrentDirectory() + @"/clientCertificates/dnvgl_expired.pfx";


            var certificateFromFile = await loadCertificate(path, "wprnbdgtq_@836-Sd#");
            foreach (var ext in certificateFromFile.Extensions)
            {
                if (ext is X509EnhancedKeyUsageExtension)
                {
                    Console.WriteLine(ext);
                }
            }
            //var certificateFromFile = await loadCertificate(path, "1234");
            Console.WriteLine(certificateFromFile.Thumbprint);

            //HttpClientSingleton.create(certificateFromFile);
            var handler = new HttpClientHandler();
            //handler.SslProtocols = SslProtocols.Tls12;
            //using var httpEventListener = new EventSourceListener("Microsoft-System-Net-Http");
            handler.ClientCertificates.Add(certificateFromFile);
            handler.CheckCertificateRevocationList = true;
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            };
            HttpClient client = new HttpClient(handler);

            var headers = new Dictionary<string, string>();
            var request = buildRequest("https://mdsconnector02.tk/neuron/bc1/test",
                        HttpMethod.Get,
                        headers);
            
            var response = client.Send(request);
            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(response.StatusCode);
            Console.WriteLine(responseString);
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
