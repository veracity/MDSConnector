using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Controllers
{
    public class la_object
    {

        public int IMO { get; set; }
        public string Date_UTC { get; set; }
        public string Time_UTC { get; set; }
        public string Event { get; set; }
        public string Voyage_From { get; set; }
        public string Voyage_To { get; set; }
        public string Latitude_Degree { get; set; }
        public string Latitude_Minutes { get; set; }
        public string Latitude_North_South { get; set; }
        public string Longitude_Degree { get; set; }
        public string Longitude_Minutes { get; set; }
        public string Longitude_East_West { get; set; }
        public string Time_Since_Previous_Report { get; set; }
        public string Time_Elapsed_Anchoring { get; set; }
        public string Distance { get; set; }
        public int Cargo_Mt { get; set; }
        public double ME_Consumption_MGO { get; set; }
        public double AE_Consumption_MGO { get; set; }
        public double Boiler_Consumption_MGO { get; set; }
        public double MGOROB { get; set; }
    }

}


