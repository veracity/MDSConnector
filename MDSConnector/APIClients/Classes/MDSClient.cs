using MDSConnector.Utilities.ConfigHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MDSConnector.APIClients
{
    public class MDSClient : IMDSClient
    {
        private readonly ILogger<MDSClient> _logger;
        private readonly HttpClient _client;
        private readonly MDSConfig _config;
        //private JwtSecurityToken _token = null;
        private JwtSecurityToken _token = new JwtSecurityToken("eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJ2ZXJhY2l0eSIsImF1dGhvcml0aWVzIjpbIlJPTEVfVVNFUiJdLCJpYXQiOjE1ODE2OTI0NjksImV4cCI6MTU4MTc3ODg2OX0.j86omnlUFKDcR_sRkXZATA10mZIn8j240_tq7D16PmK2nxuGflxR-ezDCLA8eqLd-H7Lm-UaoUZQC44Ig_DwBA");


        public MDSClient(ILogger<MDSClient> logger, HttpClient client, IOptions<MDSConfig> config)
        {
            _logger = logger;
            _client = client;
            _config = config.Value;
        }

        public async Task<string> GetVesselNames()
        {
            if (_token == null)
            {
                await GetAuthToken();
            }


            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.baseUrl + "/connector/navtor/getvesselnames");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.RawData);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;

        }

        public async Task<string> GetInfrastructure()
        {
            if (_token == null)
            {
                await GetAuthToken();
            }

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(_config.baseUrl + "/connector/navtor/infrastructure");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.RawData);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;

        }


        private async Task<bool> GetAuthToken()
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
            response.EnsureSuccessStatusCode();

            var rawTokens = response.Headers.GetValues("Authorization").ToList();
            _logger.LogError(rawTokens[0]);
            _token = new JwtSecurityToken(rawTokens[0].Split(' ')[1]);


            return true;
        }



    }
}
