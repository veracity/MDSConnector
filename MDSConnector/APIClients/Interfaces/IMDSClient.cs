using MDSConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.APIClients
{
    public interface IMDSClient
    {
        public Task<List<VesselNameModel>> GetVesselNames();
        public Task<string> GetInfrastructure();
    }
}
