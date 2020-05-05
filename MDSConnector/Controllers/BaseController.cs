using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using MDSConnector.APIClients;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Runtime.Serialization.Json;
using MDSConnector.Authentication;

namespace MDSConnector.Controllers
{

    //<summary>
    //The base controller for the connector
    //</summary>
    [ApiController]
    [Route("")]
    public class BaseController: ControllerBase
    {
        private readonly ILogger<BaseController> _logger;
        private readonly IMDSClient _mdsClient;
        private readonly IAzureStorageClient _azureStorageClient;
        private static readonly HashSet<string> supportedFormats = new HashSet<string> { "xml", "json" };

        public BaseController(ILogger<BaseController> logger, IMDSClient mdsClient, IAzureStorageClient azureStorageClient)
        {
            _logger = logger;
            _mdsClient = mdsClient;
            _azureStorageClient = azureStorageClient;
        }


        //<summary>
        //Endpoint used for demonstrating different claims are issued to different client certificates
        //</summary>
        [HttpGet]
        [CertificateAuthorized]
        public string Get()
        {
            var claims = HttpContext.User.Claims;
            var identity = HttpContext.User.Identity;
            StringBuilder builder = new StringBuilder();
            foreach (var claim in claims)
            {
                builder.Append(claim.Type);
                builder.Append("\t");
                builder.Append(claim.Value);
                builder.Append("\n");
            }
            return builder.ToString();
        }

        //<summary>
        //Endpoint used for demonstrating admin claim is only given to the client that has its certificate recognized
        //as an admin certificate
        //</summary>
        [HttpGet]
        [Route("/admin")]
        [AdminAuthorized]
        public string adminPage()
        {
            return "You are an admin!";
        }


        //<summary>>
        //Get data from the /getvesselnames endpoint at the MDS api. Demonstrates that the veracity connector can
        //deliver data in both xml and json.
        //</summary>
        [HttpGet]
        [Route("/getvesselnames")]
        public async Task<IActionResult> GetVesselNames([FromQuery(Name = "format")] string format)
        {

            format = format == null ? "xml" : format;
            if (!supportedFormats.Contains(format.ToLower()))
            {
                return StatusCode(406, $"format: {format} not supported for this endpoint");
            }

            var vesselNames = await _mdsClient.GetVesselNamesString();
            var content = "";

            if (format == "xml")
            {
                Response.ContentType = "text/xml";
                return new ContentResult
                        {
                            Content = vesselNames,
                            ContentType = "text/xml",
                            StatusCode = 200
                        };
                        
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(vesselNames);
                content = JsonConvert.SerializeObject(doc);
                return new ContentResult
                {
                    Content = content,
                    ContentType = "text/json",
                    StatusCode = 200
                };
            }
   
        }

        //<summary>>
        //Get data from the /infrastructure endpoint at the MDS api. Demonstrates that the veracity connector can
        //deliver data in both xml and json.
        //</summary>
        [HttpGet]
        [Route("/infrastructure")]
        public async Task<IActionResult> GetInfrastructure([FromQuery(Name = "format")] string format)
        {
            format = format == null ? "json" : format;
            if (!supportedFormats.Contains(format.ToLower()))
            {
                return StatusCode(406, $"format: {format} not supported for this endpoint");
            }
            var infrastructure = await _mdsClient.GetInfrastructure();
            var content = "";

            if (format == "xml")
            {
                var xmlString = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(
                                    Encoding.ASCII.GetBytes(infrastructure), new XmlDictionaryReaderQuotas())).ToString();
                content = xmlString;
                return new ContentResult
                {
                    Content = xmlString,
                    ContentType = "text/xml",
                    StatusCode = 200
                };
            }
            else
            {
                content = infrastructure;
                return new ContentResult
                {
                    Content = content,
                    ContentType = "text/json",
                    StatusCode = 200
                };
            }

        }


        //<summary>>
        //Get data from the /getvesselnames endpoint at the MDS api. Demonstrates that the veracity connector can
        //upload data to veracity container, in both xml and json format.
        //</summary>
        [HttpGet]
        [Route("/upload/getvesselnames")]
        public async Task<IActionResult> UploadVesselNames([FromQuery(Name = "format")] string format)
        {

            var fileName = "";
            try
            {
                format = format == null ? "xml" : format;
                if (!supportedFormats.Contains(format.ToLower()))
                {
                    return StatusCode(406, $"format: {format} not supported for this endpoint");
                }

                fileName = "vesselNames/vesselNames_" + Guid.NewGuid().ToString() + $".{format}";
                var vesselNames = await _mdsClient.GetVesselNamesString();
                string fileContent = "";
                if (format == "xml")
                {
                    fileContent = vesselNames;
                }
                else if (format == "json")
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(vesselNames);
                    fileContent = JsonConvert.SerializeObject(doc);
                }
                await _azureStorageClient.UploadStringToFile(fileName, fileContent);
            }
            catch (Exception e) { return StatusCode(500, e.Message); }

            return Ok($"File path: {fileName}");
        }

        //<summary>>
        //Get data from the /infrastructure endpoint at the MDS api. Demonstrates that the veracity connector can
        //upload data to veracity container, in both xml and json format.
        //</summary>
        [HttpGet]
        [Route("/upload/infrastructure")]
        public async Task<IActionResult> UploadInfrastructure([FromQuery(Name = "format")] string format)
        {

            var fileName = "";
            try
            {
                format = format == null ? "json" : format;
                if (!supportedFormats.Contains(format.ToLower()))
                {
                    return StatusCode(406, $"format: {format} not supported for this endpoint");
                }

                var infrastructureRes = await _mdsClient.GetInfrastructure();
                fileName = "infrastructure/infrastructure_" + Guid.NewGuid().ToString() + $".{format}";
                var fileContent = "";
                if (format == "xml")
                {
                    var xmlString = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(
                                    Encoding.ASCII.GetBytes(infrastructureRes), new XmlDictionaryReaderQuotas())).ToString();
                    fileContent = xmlString;
                }
                else if (format == "json")
                {
                    fileContent = infrastructureRes;
                }
                await _azureStorageClient.UploadStringToFile(fileName, fileContent);
            }
            catch (Exception e){ return StatusCode(500, e.Message); }

            return Ok($"File path: {fileName}");
        }
    }
}
