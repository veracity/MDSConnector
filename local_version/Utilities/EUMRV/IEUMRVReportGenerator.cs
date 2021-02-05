using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities.EUMRV
{
    public interface IEUMRVReportGenerator
    {
        public string GenerateLogabstract(Dictionary<string, string> logabstractData);

        public string GenerateBunkerReport(Dictionary<string, string> bunkerData);
    }
}
