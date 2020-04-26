using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MDSConnector.Models
{
    /// <summary>
    /// Data model used for parsing api response from MDS API, endpoint getvesselnames.
    /// </summary>
    [XmlRoot(ElementName = "VesselName", Namespace = "http://schemas.datacontract.org/2004/07/NavService")]
    public class VesselNameModel
    {
        [XmlElement(ElementName="Imo", Namespace = "http://schemas.datacontract.org/2004/07/NavService")]
        public string Imo;
        [XmlElement(ElementName="Name", Namespace= "http://schemas.datacontract.org/2004/07/NavService")]
        public string Name;

    }
}
