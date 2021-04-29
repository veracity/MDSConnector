using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.APIClients
{
    /// <summary>
    /// Interface definition for the wrapper class that communicates with azure storage.
    /// </summary>
    public interface IAzureStorageClient
    {
        public Task<bool> UploadStringToFile(string fileName, string content);

    }
}
