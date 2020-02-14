using MDSConnector.Utilities.ConfigHelpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MDSConnector.APIClients
{
    public class VeracityClient : IVeracityClient
    {

        private readonly ILogger<VeracityClient> _logger;
        private readonly HttpClient _client;
        private readonly VeracityConfig _config;
       
        public VeracityClient(ILogger<VeracityClient> logger, HttpClient client, VeracityConfig config)
        {
            _logger = logger;
            _client = client;
            _config = config;
        }


    }
}
