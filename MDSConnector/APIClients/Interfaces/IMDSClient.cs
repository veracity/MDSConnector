using MDSConnector.Models;
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
        public Task<List<VesselNameModel>> GetVesselNames();

        public Task<string> GetVesselNamesString();
        public Task<string> GetInfrastructure();
        public Task<string> GetFleet();
        public Task<string> GetRouteInfoForVessel(string vesselID);
        public Task<string> GetRouteData(string routeID);
    }
}
