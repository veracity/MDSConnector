using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities.ConfigHelpers
{
    /// <summary>
    /// Data class that contains the SAS token for uploading data to destination veracity container
    /// </summary>
    public class AzureStorageConfig
    {
        public string sasToken { get; set; }

        public override string ToString()
        {
            return $"SAS Token: {sasToken}";
        }
    }
}
