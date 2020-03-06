using MDSConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.APIClients
{
    public interface IAzureStorageClient
    {
        public Task<bool> UploadVesselNames(string fileName, List<VesselNameModel> vesselNames);

        public Task<bool> UploadStringToFile(string fileName, string content);

    }
}
