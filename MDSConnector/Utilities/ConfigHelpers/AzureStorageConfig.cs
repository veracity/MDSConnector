using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities.ConfigHelpers
{
    public class AzureStorageConfig
    {
        public string sasToken { get; set; }

        public override string ToString()
        {
            return $"SAS Token: {sasToken}";
        }
    }
}
