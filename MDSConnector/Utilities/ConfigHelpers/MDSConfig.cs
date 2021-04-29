using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities.ConfigHelpers
{
    //<summary>
    //This is the data class that maps from the appsettings/MDSConfig
    //Information that is needed for communicating with the MDS API.
    //</summary>
    public class MDSConfig
    {
        public string NeuronUrl { get; set; }
        
        public override string ToString()
        {
            return $"baseUrl {NeuronUrl}";
        }
    }

}
