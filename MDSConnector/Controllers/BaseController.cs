using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Certificate;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using MDSConnector.Utilities.ConfigHelpers;
using Microsoft.Extensions.Options;
using MDSConnector.APIClients;
using MDSConnector.Models;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Runtime.Serialization.Json;
using System.Net;

namespace MDSConnector.Controllers
{
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

        [HttpGet]
        [Authorize]
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

        [HttpGet]
        //[Authorize]
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
                //content = vesselNames;
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


        [HttpGet]
        //[Authorize]
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
