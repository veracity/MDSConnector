using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDSConnector.Utilities.EUMRV
{

    //<summary>
    //This class should implement logic for generating an EUMRV report
    //Additional data classes should be implemented aswell if needed (parsing raw response data from MDSapi to workable objects).
    //</summary>
    public class BasicEUMRVReportGenerator : IEUMRVReportGenerator
    {

        //<summary>
        //Generate the report using this method. It should be stateless and take in all neccesary data from the parameters
        //</summary>
        public string GenerateLogabstract(List<Dictionary<string, string>> logabstractData)
        {
            var reportBuilder = new StringBuilder();
            var firstEntry = logabstractData[0];
            var keys = firstEntry.Select(x => x.Key);

            reportBuilder.Append(string.Join(',', keys));
            foreach (var entry in logabstractData)
            {
                var data = entry.Select(x => x.Value);
                reportBuilder.Append("\n");
                reportBuilder.Append(string.Join(',', data));
            }

            return reportBuilder.ToString();
        }

        public string GenerateBunkerReport(Dictionary<string, string> bunkerData)
        {
            var reportBuilder = new StringBuilder();

            var entries = bunkerData.ToList();
            var keys = entries.Select(x => x.Key);
            var data = entries.Select(x => x.Value);

            reportBuilder.Append(string.Join(',', keys));
            reportBuilder.Append("\n");
            reportBuilder.Append(string.Join(',', data));

            return reportBuilder.ToString();
        }




    }
}
