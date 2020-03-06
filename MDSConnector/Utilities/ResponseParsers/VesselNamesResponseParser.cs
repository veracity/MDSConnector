using MDSConnector.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MDSConnector.Utilities.ResponseParsers
{
    public class VesselNamesResponseParser
    {

        public static async Task<List<VesselNameModel>> Parse(HttpResponseMessage httpResponse)
        {
            List<VesselNameModel> vesselNames = new List<VesselNameModel>();
            var contentType = httpResponse.Content.Headers.ContentType.MediaType;

            if (contentType == "text/xml")
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseContent);

                var serializer = new XmlSerializer(typeof(VesselNameModel));
                var vesselNameNodes = xmlDoc.GetElementsByTagName("a:VesselName");
                foreach (XmlNode vesselNameNode in vesselNameNodes)
                {
                    var vesselName = (VesselNameModel) serializer.Deserialize(new StringReader(vesselNameNode.OuterXml));
                    vesselNames.Add(vesselName);
                }
                return vesselNames;
            }
            throw new NotImplementedException($"VesselNames Response Content type: {contentType} not supported.");
        }



    }
}
