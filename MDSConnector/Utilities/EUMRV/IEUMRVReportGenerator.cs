using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities.EUMRV
{
    public interface IEUMRVReportGenerator
    {
        public string GenerateLogabstract(List<Dictionary<string, string>> logabstractData);

        public string GenerateBunkerReport(List<Dictionary<string, string>> bunkerData);
    }
}
