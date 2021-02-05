using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Controllers
{
    public class Rootobject
    {
        public string imo { get; set; }
        public DateTime dateUtc { get; set; }
        public string timeUtc { get; set; }
        public string voyageFrom { get; set; }
        public string voyageTo { get; set; }
        public string latitudeDegree { get; set; }
        public string latitudeMinutes { get; set; }
        public string latitudeNorthSouth { get; set; }
        public string longitudeDegree { get; set; }
        public string longitudeMinutes { get; set; }
        public string longitudeEastWest { get; set; }
        public int windForceBft { get; set; }
        public string _event { get; set; }
        public float timeSincePreviousReport { get; set; }
        public int timeElapsedAnchoring { get; set; }
        public float distance { get; set; }
        public int cargoM3 { get; set; }
        public object mEcConsumptionHFO { get; set; }
        public object meConsumptionLFO { get; set; }
        public float? mEcConsumptionMGO { get; set; }
        public object mEcConsumptionMDO { get; set; }
        public int aeConsumptionHFO { get; set; }
        public int aeConsumptionLFO { get; set; }
        public int aeConsumptionMGO { get; set; }
        public int aeConsumptionMDO { get; set; }
        public object boilerConsumptionHFO { get; set; }
        public object boilerConsumptionLFO { get; set; }
        public float boilerConsumptionMGO { get; set; }
        public object boilerConsumptionMDO { get; set; }
        public int inertGasConsumptionHFO { get; set; }
        public int inertGasConsumptionLFO { get; set; }
        public int inertGasConsumptionMGO { get; set; }
        public int inertGasConsumptionMDO { get; set; }
        public int hforob { get; set; }
        public float lforob { get; set; }
        public float mgorob { get; set; }
        public int mdorob { get; set; }
        public object meConsumptionMDO { get; set; }
        public float meConsumptionMGO { get; set; }
        public object meConsumptionHFO { get; set; }
    }
}
