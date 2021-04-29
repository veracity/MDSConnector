using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MDSClient
{
    /// <summary>
    /// Singleton implementation of a httpclient that uses a X509Certificate for authenticating requests.
    /// </summary>
    sealed class HttpClientSingleton
    {
        private static HttpClientSingleton _instance = null;
        private static HttpClient client;
        private static HttpClientHandler handler;

        private HttpClientSingleton(){
            throw new Exception("This contructor should not be used");
        }


        private HttpClientSingleton(X509Certificate2 certificate)
        {
            var clientHandler = new HttpClientHandler();
            clientHandler.CheckCertificateRevocationList = false;
            clientHandler.ClientCertificates.Add(certificate);
            clientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
            clientHandler.ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            };
            handler = clientHandler;
            client = new HttpClient(clientHandler);
        }

        public static HttpClientSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new Exception("HttpClientSingleton object not created");
                }
                return _instance;
            }
        }

        public static void create(X509Certificate2 certificate)
        {
            if (_instance != null)
            {
                throw new Exception("Object already created");
            }
            _instance = new HttpClientSingleton(certificate);
        }

        public static void setCertificate(X509Certificate2 newCertificate)
        {
            var clientHandler = new HttpClientHandler();
            clientHandler.ClientCertificates.Add(newCertificate);
            clientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
            clientHandler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };
            client = new HttpClient(clientHandler);
        }

        public Task<HttpResponseMessage> sendAsync(HttpRequestMessage request)
        {
            return client.SendAsync(request);
        }

    }
}
