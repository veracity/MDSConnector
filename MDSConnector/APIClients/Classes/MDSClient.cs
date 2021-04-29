using MDSConnector.Utilities.ConfigHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MDSConnector.APIClients
{
    /// <summary>
    /// Wrapper class that incapsulates the communication with MDS API
    /// Incapsulates a httpClient and contains all the config information required for communication
    /// 
    /// This client is authenticated with the MDS api using a JWTSecurity token, obtained by calling the private method "GetAuthToken".
    /// Detailed authentication process with MDS APi is described in another document.
    /// 
    /// Token expiration is not likely in the current usecases, and therefore is not implemented. Should later usecases require this, it should be implemented in this class.
    /// 
    /// </summary>
    public class MDSClient : IMDSClient
    {
        private readonly HttpClient _client;
        private readonly MDSConfig _config;

        public MDSClient( HttpClient client, MDSConfig config)
        {
            _client = client;
            _config = config;
        }

        /// <summary>
        /// Method that communicates with the Navtor logabstract endppoint
        /// </summary>
        //public async Task<string> GetLogAbstractNavtor()
        //{
        //    if (_token == null) {   await GetAuthToken();   }

        //    var request = new HttpRequestMessage();
        //    request.Method = HttpMethod.Get;
        //    request.RequestUri = new Uri(_config.baseUrl + "/navtor/mock/api/logabstract");
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.RawData);

        //    var response = await _client.SendAsync(request);
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        throw new Exception("Get logabstract Navtor failed");
        //    }
        //    return await response.Content.ReadAsStringAsync();
        //}

        public async Task<string> PingNeuron()
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.NeuronUrl + "/neuron/bc1/test");

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Ping Neuron failed");
            }
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Method that communicates with the Neuron logabstract endppoint
        /// </summary>
        public async Task<string> GetLogAbstractNeuron()
        {

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.NeuronUrl + "/neuron/bc1/data");

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Get logabstract Neuron failed");
            }
            return await response.Content.ReadAsStringAsync();
        }



    }
}
