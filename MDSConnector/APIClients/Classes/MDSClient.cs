using MDSConnector.Utilities.ConfigHelpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
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
        private string _token;


        public MDSClient(ILogger<MDSClient> logger, HttpClient client, MDSConfig config)
        {
            _logger = logger;
            _client = client;
            _config = config;
        }

        public async Task<string> GetVesselNames()
        {
            if (_token == null)
            {
                await GetAuthToken();
            }

            return _token;


        }

        private async Task<bool> GetAuthToken()
        {
            var requestBody = new Dictionary<string, string>
            {
                {"username",  _config.username },
                {"password", _config.password }
            };
            var serializedContent = new StringContent(
                                JsonConvert.SerializeObject(requestBody),
                                Encoding.UTF8,
                                "applicaiton/json");

            var response = await _client.PostAsync(_config.baseUrl + "/auth", serializedContent);
            response.EnsureSuccessStatusCode();

            var rawTokens = response.Headers.GetValues("Authorization").ToList();
            foreach (var rawToken in rawTokens)
            {
                Console.WriteLine(rawToken);
            }
            _token = rawTokens[0];

            return true;
        }



    }
}
