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
    /// <summary>
    /// Wrapper class for communication with azure storage
    /// uses Microsoft pacakge Microsoft.Azure.Storage.Blob
    /// Documentation: https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
    /// </summary>
    public class AzureStorageClient : IAzureStorageClient
    {

        private readonly ILogger<IAzureStorageClient> _logger;
        private readonly HttpClient _client;
        private readonly AzureStorageConfig _config;
       

        public AzureStorageClient(ILogger<IAzureStorageClient> logger, HttpClient client, AzureStorageConfig config)
        {
            _logger = logger;
            _client = client;
            _config = config;
        }



        /// <summary>
        /// Generic method that uploads string to a blob at an azure storage location
        /// </summary>
        public async Task<bool> UploadStringToFile(string fileName, string content)
        {
            var container = new CloudBlobContainer(new Uri(_config.sasToken));
            try
            {
                var blob = container.GetBlockBlobReference(fileName);
                await blob.UploadTextAsync(content);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
