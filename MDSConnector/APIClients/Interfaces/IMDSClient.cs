using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.APIClients
{
    /// <summary>
    /// Interface definition for the wrapper class that communicates with the MDS api.
    /// </summary>
    public interface IMDSClient
    {

        //public Task<string> GetLogAbstractNavtor();

        public Task<string> GetLogAbstractNeuron();

        public Task<string> PingNeuron();

        //public Task<string> GetBunkerWilhemsen();
    }
}
