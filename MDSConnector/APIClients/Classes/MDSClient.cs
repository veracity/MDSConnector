using MDSConnector.Models;
using MDSConnector.Utilities.ConfigHelpers;
using MDSConnector.Utilities.ResponseParsers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

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
        private readonly ILogger<MDSClient> _logger;
        private readonly HttpClient _client;
        private readonly MDSConfig _config;
        private JwtSecurityToken _token = null;


        public MDSClient(ILogger<MDSClient> logger, HttpClient client, IOptions<MDSConfig> config)
        {
            _logger = logger;
            _client = client;
            _config = config.Value;
        }

        /// <summary>
        /// Method that communicates with the Navtor logabstract endppoint
        /// </summary>
        public async Task<string> GetLogAbstractNavtor()
        {
            if (_token == null) {   await GetAuthToken();   }

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.baseUrl + "/navtor/mock/api/logabstract");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.RawData);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Get logabstract Navtor failed");
            }
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Method that communicates with the Neuron logabstract endppoint
        /// </summary>
        public async Task<string> GetLogAbstractNeuron()
        {
            if (_token == null) { await GetAuthToken(); }

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.baseUrl + "/neuron/mock/api/logabstract");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.RawData);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Get logabstract Neuron failed");
            }
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Method that communicates with the Wilhemsen bunker report endppoint
        /// </summary>
        public async Task<string> GetBunkerWilhemsen()
        {
            if (_token == null) { await GetAuthToken(); }

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.baseUrl + "/wsm/mock/api/bunker");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.RawData);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Get Bunker Wilhemsen failed {await response.Content.ReadAsStringAsync()}");
            }
            return await response.Content.ReadAsStringAsync();
        }


        /// <summary>
        /// Method that communicates with the getvesselnames endpoint (return as parsed data models)
        /// </summary>
        public async Task<List<VesselNameModel>> GetVesselNames()
        {
            if (_token == null) {   await GetAuthToken();   }


            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.baseUrl + "/connector/navtor/getvesselnames");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.RawData);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)  
            {
                throw new Exception("Get vessel names failed");
            }

            return await VesselNamesResponseParser.Parse(response);
        }
        /// <summary>
        /// Method that communicates with the GetVesselnames endpoint (return as string)
        /// </summary>
        public async Task<string> GetVesselNamesString()
        {
            if (_token == null) { await GetAuthToken(); }


            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.baseUrl + "/connector/navtor/getvesselnames");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.RawData);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Get vessel names failed");
            }

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Method that communicates with the GetInfrastructure endpoint
        /// </summary>
        public async Task<string> GetInfrastructure()
        {
            if (_token == null) {   await GetAuthToken();   }

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.baseUrl + "/connector/navtor/infrastructure");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.RawData);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Get infrastructure failed");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent = responseContent.Replace(@"\n", "").Replace(@"\", "");
            responseContent = responseContent.Substring(1, responseContent.Length - 2);

            return responseContent;

        }

        /// <summary>
        /// Method that communicates with the GetFleet endpoint
        /// </summary>
        public async Task<string> GetFleet()
        {
            if (_token == null) { await GetAuthToken();  }

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.baseUrl + "/connector/navtor/getfleet");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.RawData);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Get fleet failed");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent = responseContent.Replace(@"\n", "").Replace(@"\", "");
            responseContent = responseContent.Substring(1, responseContent.Length - 2);

            return responseContent;


        }

        /// <summary>
        /// Method that communicates with the GetRouteInfoForVessel endpoint
        /// </summary>
        public async Task<string> GetRouteInfoForVessel(string vesselID)
        {
            if (_token == null) { await GetAuthToken(); }

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.baseUrl + "/connector/navtor/getrouteinfoforvessel/" + vesselID);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.RawData);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Get route info for vessel: {vesselID} failed");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;

        }

        /// <summary>
        /// Method that communicates with the getRouteData endpoint
        /// </summary>
        public async Task<string> GetRouteData(string routeID)
        {

            if (_token == null) { await GetAuthToken(); }

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.baseUrl + "/connector/navtor/getroutedata/" + routeID);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.RawData);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Get route data for route: {routeID} failed");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;

        }

        /// <summary>
        /// Private method that gets and sets the jwt for MDS API.
        /// </summary>
        /// <returns></returns>
        private async Task GetAuthToken()
        {
            dynamic requestBody = new ExpandoObject();
            requestBody.username = _config.username;
            requestBody.password = _config.password;
            string serializedBody = JsonConvert.SerializeObject(requestBody);

            var serializedContent = new StringContent(
                                serializedBody,
                                Encoding.UTF8,
                                "applicaiton/json");

            var response = await _client.PostAsync(_config.baseUrl + "/auth", serializedContent);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Get authorization token failed {await response.Content.ReadAsStringAsync()}");
            }

            var rawTokens = response.Headers.GetValues("Authorization").ToList();
            _token = new JwtSecurityToken(rawTokens[0].Split(' ')[1]);
        }
    }
}
