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

            return await response.Content.ReadAsStringAsync(); ;
        }


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
                throw new Exception("Get authorization token failed");
            }

            var rawTokens = response.Headers.GetValues("Authorization").ToList();
            _token = new JwtSecurityToken(rawTokens[0].Split(' ')[1]);
        }



    }
}
