using MDSConnector.Models;
using MDSConnector.Utilities.ConfigHelpers;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MDSConnector.APIClients
{
    public class AzureStorageClient : IAzureStorageClient
    {

        private readonly ILogger<IAzureStorageClient> _logger;
        private readonly HttpClient _client;
        private readonly AzureStorageConfig _config;
       
        public AzureStorageClient(ILogger<IAzureStorageClient> logger, HttpClient client, IOptions<AzureStorageConfig> config)
        {
            _logger = logger;
            _client = client;
            _config = config.Value;
        }

        public async Task<bool> UploadVesselNames(string fileName, List<VesselNameModel> vesselNames)
        {
            var container = new CloudBlobContainer(new Uri(_config.sasToken));
            
            var fileContent = JsonConvert.SerializeObject(vesselNames);
            try
            {
                var blob = container.GetBlockBlobReference(fileName);
                await blob.UploadTextAsync(fileContent);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
