using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using MDSConnector.APIClients;
using System.Xml;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization.Json;
using MDSConnector.Authentication;
using MDSConnector.Utilities.EUMRV;
using MDSConnector.Utilities;
using System.IO;

namespace MDSConnector.Controllers
{

    //<summary>
    //The base controller for the connector
    //</summary>
    [ApiController]
    [Route("")]
    public class BaseController : ControllerBase
    {
        private readonly ILogger<BaseController> _logger;
        private readonly IMDSClient _mdsClient;
        private readonly IAzureStorageClient _azureStorageClient;
        private readonly IEUMRVReportGenerator _eumrvReportGenerator;
        private readonly ITimeProvider _timeProvider;
        private static readonly HashSet<string> supportedFormats = new HashSet<string> { "xml", "json" };

        public BaseController(ILogger<BaseController> logger, IMDSClient mdsClient, IAzureStorageClient azureStorageClient, IEUMRVReportGenerator eumrvReportGenerator, ITimeProvider timeProvider)
        {
            _logger = logger;
            _mdsClient = mdsClient;
            _azureStorageClient = azureStorageClient;
            _eumrvReportGenerator = eumrvReportGenerator;
            _timeProvider = timeProvider;
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


        /// <summary>
        /// Endpoint used for triggering EUMRV report generating feature
        /// </summary>
        //[HttpGet]
        // [Route("/eumrv")]
        //[CertificateAuthorized]
        // public async Task<IActionResult> GenerateEUMRVReport()
        // {
        //var asyncRequests = new Dictionary<string, Task<string>>();
        //asyncRequests["bunker"] =  _mdsClient.GetBunkerWilhemsen();
        // asyncRequests["LANavtor"] = _mdsClient.GetLogAbstractNavtor();
        //
        // asyncRequests["LANeuron"] = _mdsClient.GetLogAbstractNeuron();
        //var asyncRequests = new Dictionary<string, string>();






        /*var results = new Dictionary<string, string>();
        foreach (var item in asyncRequests)
        {
            results[item.Key] = await item.Value;
        }*/




        //var bunkerData = JsonConvert.DeserializeObject<Dictionary<string, string>>(results["bunker"]);

        //var logabstractData = new Dictionary<string, string>();
        // var LANavtor = JsonConvert.DeserializeObject<Dictionary<string, string>>(results["LANavtor"]);
        // var LANeuron = JsonConvert.DeserializeObject<Dictionary<string, string>>(results["LANeuron"]);



        //var list = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(asyncRequests["LANeuron"]);
        // var dictionary = list.ToDictionary(x => x.Key, x => x.Value);



        // LANeuron.ToList().ForEach(x => logabstractData[x.Key] = x.Value);

        //var laReport = _eumrvReportGenerator.GenerateLogabstract(logabstractData);
        //var bunkerReport = _eumrvReportGenerator.GenerateBunkerReport(bunkerData);

        //var now = _timeProvider.GetNow();

        //var fileName = $"{now.Year}{now.Month}{now.Day}_{Guid.NewGuid()}";

        //var laName = $"{fileName}_la.csv";
        //var bunkerName = $"{fileName}_bn.csv";
        //var laSuccess = await _azureStorageClient.UploadStringToFile($"upload/{laName}", laReport);
        //var bunkerSuccess = await _azureStorageClient.UploadStringToFile($"upload/{bunkerName}", bunkerReport);

        //if (bunkerSuccess && laSuccess)
        /*if (laSuccess)
        {
            return Ok();
        }
        else
        {
            return StatusCode(500);
        }*/

        // }


        //<summary>>
        //Get data from the /getvesselnames endpoint at the MDS api. Demonstrates that the veracity connector can
        //deliver data in both xml and json.
        //</summary>
        //[HttpGet]
        //[Route("/getvesselnames")]
        //public async Task<IActionResult> GetVesselNames([FromQuery(Name = "format")] string format)
        //{

        //    format = format == null ? "xml" : format;
        //    if (!supportedFormats.Contains(format.ToLower()))
        //    {
        //        return StatusCode(406, $"format: {format} not supported for this endpoint");
        //    }

        //    var vesselNames = await _mdsClient.GetVesselNamesString();
        //    var content = "";

        //    if (format == "xml")
        //    {
        //        Response.ContentType = "text/xml";
        //        return new ContentResult
        //                {
        //                    Content = vesselNames,
        //                    ContentType = "text/xml",
        //                    StatusCode = 200
        //                };

        //    }
        //    else
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.LoadXml(vesselNames);
        //        content = JsonConvert.SerializeObject(doc);
        //        return new ContentResult
        //        {
        //            Content = content,
        //            ContentType = "text/json",
        //            StatusCode = 200
        //        };
        //    }

        //}

        //<summary>>
        //Get data from the /infrastructure endpoint at the MDS api. Demonstrates that the veracity connector can
        //deliver data in both xml and json.
        //</summary>
        //[HttpGet]
        //[Route("/infrastructure")]
        //public async Task<IActionResult> GetInfrastructure([FromQuery(Name = "format")] string format)
        //{
        //    format = format == null ? "json" : format;
        //    if (!supportedFormats.Contains(format.ToLower()))
        //    {
        //        return StatusCode(406, $"format: {format} not supported for this endpoint");
        //    }
        //    var infrastructure = await _mdsClient.GetInfrastructure();
        //    var content = "";

        //    if (format == "xml")
        //    {
        //        var xmlString = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(
        //                            Encoding.ASCII.GetBytes(infrastructure), new XmlDictionaryReaderQuotas())).ToString();
        //        content = xmlString;
        //        return new ContentResult
        //        {
        //            Content = xmlString,
        //            ContentType = "text/xml",
        //            StatusCode = 200
        //        };
        //    }
        //    else
        //    {
        //        content = infrastructure;
        //        return new ContentResult
        //        {
        //            Content = content,
        //            ContentType = "text/json",
        //            StatusCode = 200
        //        };
        //    }

        //}


        //<summary>>
        //Get data from the /getvesselnames endpoint at the MDS api. Demonstrates that the veracity connector can
        //upload data to veracity container, in both xml and json format.
        //</summary>
        //[HttpGet]
        //[Route("/upload/getvesselnames")]
        //public async Task<IActionResult> UploadVesselNames([FromQuery(Name = "format")] string format)
        //{

        //    var fileName = "";
        //    try
        //    {
        //        format = format == null ? "xml" : format;
        //        if (!supportedFormats.Contains(format.ToLower()))
        //        {
        //            return StatusCode(406, $"format: {format} not supported for this endpoint");
        //        }

        //        fileName = "vesselNames/vesselNames_" + Guid.NewGuid().ToString() + $".{format}";
        //        var vesselNames = await _mdsClient.GetVesselNamesString();
        //        string fileContent = "";
        //        if (format == "xml")
        //        {
        //            fileContent = vesselNames;
        //        }
        //        else if (format == "json")
        //        {
        //            XmlDocument doc = new XmlDocument();
        //            doc.LoadXml(vesselNames);
        //            fileContent = JsonConvert.SerializeObject(doc);
        //        }
        //        await _azureStorageClient.UploadStringToFile(fileName, fileContent);
        //    }
        //    catch (Exception e) { return StatusCode(500, e.Message); }

        //    return Ok($"File path: {fileName}");
        //}

        //<summary>>
        //Get data from the /infrastructure endpoint at the MDS api. Demonstrates that the veracity connector can
        //upload data to veracity container, in both xml and json format.
        //</summary>
        //[HttpGet]
        //[Route("/upload/infrastructure")]
        //public async Task<IActionResult> UploadInfrastructure([FromQuery(Name = "format")] string format)
        //{

        //    var fileName = "";
        //    try
        //    {
        //        format = format == null ? "json" : format;
        //        if (!supportedFormats.Contains(format.ToLower()))
        //        {
        //            return StatusCode(406, $"format: {format} not supported for this endpoint");
        //        }

        //        var infrastructureRes = await _mdsClient.GetInfrastructure();
        //        fileName = "infrastructure/infrastructure_" + Guid.NewGuid().ToString() + $".{format}";
        //        var fileContent = "";
        //        if (format == "xml")
        //        {
        //            var xmlString = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(
        //                            Encoding.ASCII.GetBytes(infrastructureRes), new XmlDictionaryReaderQuotas())).ToString();
        //            fileContent = xmlString;
        //        }
        //        else if (format == "json")
        //        {
        //            fileContent = infrastructureRes;
        //        }
        //        await _azureStorageClient.UploadStringToFile(fileName, fileContent);
        //    }
        //    catch (Exception e){ return StatusCode(500, e.Message); }

        //    return Ok($"File path: {fileName}");
        //}
        [HttpGet]   //get from localhost
        [Route("/eumrv3")]
        public async Task<IActionResult> GetMRV()
        {

            string filePath = System.IO.File.ReadAllText("App_Data/la_tijuca.json");
            string filePath1 = System.IO.File.ReadAllText("App_Data/bn_tijuca.json");



            var myList = JsonConvert.DeserializeObject<List<la_object>>(filePath);
            var myList1 = JsonConvert.DeserializeObject<List<bn_object>>(filePath1);

            


            Dictionary<string, string> fooDict = new Dictionary<string, string>();
            foreach (la_object foo in myList)
            {
                fooDict["IMO"] = foo.IMO.ToString();
                fooDict["Date_UTC"] = foo.Date_UTC.ToString();
                fooDict["Time_UTC"] = foo.Time_UTC.ToString();
                fooDict["Voyage_From"] = foo.Voyage_From;
                fooDict["Voyage_To"] = foo.Voyage_To;
                fooDict["Latitude_Degree"] = foo.Latitude_Degree.ToString();
                fooDict["Latitude_Minutes"] = foo.Latitude_Minutes.ToString();
                fooDict["Latitude_North_South"] = foo.Latitude_North_South;
                fooDict["Longitude_Degree"] = foo.Longitude_Degree.ToString();
                fooDict["Longitude_Minutes"] = foo.Longitude_Minutes.ToString();
                fooDict["Longitude_East_West"] = foo.Longitude_East_West;
                fooDict["Time_Since_Previous_Report"] = foo.Time_Since_Previous_Report.ToString();
                fooDict["Time_Elapsed_Anchoring"] = foo.Time_Elapsed_Anchoring;
                fooDict["Distance"] = foo.Distance.ToString();
                fooDict["Cargo_Mt"] = foo.Cargo_Mt.ToString();
                fooDict["ME_Consumption_MGO"] = foo.ME_Consumption_MGO.ToString();
                fooDict["AE_Consumption_MGO"] = foo.AE_Consumption_MGO.ToString();
                fooDict["Boiler_Consumption_MGO"] = foo.Boiler_Consumption_MGO.ToString();
                fooDict["MGOROB"] = foo.MGOROB.ToString();
               

               

                var laReport = _eumrvReportGenerator.GenerateLogabstract(fooDict);
                var now = _timeProvider.GetNow();

                var fileName = $"{now.Year}{now.Month}{now.Day}_{Guid.NewGuid()}";

                var laName = $"{fileName}_la.csv";

                var laSuccess = await _azureStorageClient.UploadStringToFile($"upload/{laName}", laReport);


                
            }

            Dictionary<string, string> fooDict1 = new Dictionary<string, string>();

            foreach (bn_object fo in myList1)
            {
                fooDict1["BDN_Number"] = fo.BDN_Number.ToString();
                fooDict1["Bunker_Delivery_Date"] = fo.Bunker_Delivery_Date.ToString();
                fooDict1["IMO"] = fo.IMO.ToString();
                fooDict1["Mass"] = fo.Mass.ToString();
                fooDict1["Fuel_Type"] = fo.Fuel_Type.ToString();

                var bnReport = _eumrvReportGenerator.GenerateBunkerReport(fooDict1);
                var now1 = _timeProvider.GetNow();

                var fileName1 = $"{now1.Year}{now1.Month}{now1.Day}_{Guid.NewGuid()}";

                var laName1 = $"{fileName1}_bn.csv";

                var laSuccess1 = await _azureStorageClient.UploadStringToFile($"upload/{laName1}", bnReport);

            }

            return Ok();
        }
        /*[HttpGet]
        [Route("/eumrv2")]
        public async Task<IActionResult> GetNeuron()
        {

            string filePath = System.IO.File.ReadAllText("App_Data/sample_data.json");
            Console.WriteLine(filePath);
            var myList = JsonConvert.DeserializeObject<List<Rootobject>>(filePath);
            foreach (Rootobject item in myList)
                Console.WriteLine(item.voyageFrom);
            Dictionary<string, string> fooDict = new Dictionary<string, string>();
            foreach (Rootobject foo in myList)
            {
                fooDict["imo"] = foo.imo;
                fooDict["dateUtc"] = foo.dateUtc.ToString();
                fooDict["timeUtc"] = foo.timeUtc.ToString();
                fooDict["voyageFrom"] = foo.voyageFrom;
                fooDict["voyageTo"] = foo.voyageTo;
                fooDict["latitudeDegree"] = foo.latitudeDegree;
                fooDict["latitudeMinutes"] = foo.latitudeMinutes;
                fooDict["latitudeNorthSouth"] = foo.latitudeNorthSouth;
                fooDict["longitudeDegree"] = foo.longitudeDegree;
                fooDict["longitudeMinutes"] = foo.longitudeMinutes;
                fooDict["longitudeEastWest"] = foo.longitudeEastWest;
                fooDict["windForceBft"] = foo.windForceBft.ToString();
                fooDict["event"] = foo._event;
                fooDict["timeSincePreviousReport"] = foo.timeSincePreviousReport.ToString();
                fooDict["timeElapsedAnchoring"] = foo.timeElapsedAnchoring.ToString();
                fooDict["distance"] = foo.distance.ToString();
                fooDict["cargoM3"] = foo.cargoM3.ToString();
                if (foo.mEcConsumptionHFO == null) { fooDict["mEcConsumptionHFO"] = ""; }
                else { fooDict["mEcConsumptionHFO"] = foo.mEcConsumptionHFO.ToString(); }
                if (foo.meConsumptionLFO == null) { fooDict["meConsumptionLFO"] = ""; }
                else { fooDict["meConsumptionLFO"] = foo.meConsumptionLFO.ToString(); }
                if (foo.mEcConsumptionMGO == null)
                { fooDict["mEcConsumptionMGO"] = ""; }
                else { fooDict["mEcConsumptionMGO"] = foo.mEcConsumptionMGO.ToString(); }
                if (foo.mEcConsumptionMDO == null)
                { fooDict["mEcConsumptionMDO"] = ""; }
                else { fooDict["mEcConsumptionMDO"] = foo.mEcConsumptionMDO.ToString(); }


                if (foo.aeConsumptionHFO.ToString() == null)
                { fooDict["aeConsumptionHFO"] = ""; }
                else { fooDict["aeConsumptionHFO"] = foo.aeConsumptionHFO.ToString(); }
                if (foo.aeConsumptionLFO.ToString() == null)
                { fooDict["aeConsumptionLFO"] = ""; }
                else { fooDict["aeConsumptionLFO"] = foo.aeConsumptionLFO.ToString(); }
                if (foo.aeConsumptionMGO.ToString() == null)
                { fooDict["aeConsumptionMGO"] = ""; }
                else { fooDict["aeConsumptionMGO"] = foo.aeConsumptionMGO.ToString(); }
                if (foo.aeConsumptionMDO.ToString() == null)
                { fooDict["aeConsumptionMDO"] = ""; }
                else { fooDict["aeConsumptionMDO"] = foo.aeConsumptionMDO.ToString(); }





                if (foo.boilerConsumptionHFO == null)
                { fooDict["boilerConsumptionHFO"] = ""; }
                else { fooDict["boilerConsumptionHFO"] = foo.boilerConsumptionHFO.ToString(); }
                if (foo.boilerConsumptionLFO == null)
                { fooDict["boilerConsumptionLFO"] = ""; }
                else { fooDict["boilerConsumptionLFO"] = foo.boilerConsumptionLFO.ToString(); }
                fooDict["boilerConsumptionMGO"] = foo.boilerConsumptionMGO.ToString();
                if (foo.boilerConsumptionMDO == null)
                { fooDict["boilerConsumptionMDO"] = ""; }
                else { fooDict["boilerConsumptionMDO"] = foo.boilerConsumptionMDO.ToString(); }

                fooDict["inertGasConsumptionHFO"] = foo.inertGasConsumptionHFO.ToString();
                fooDict["inertGasConsumptionLFO"] = foo.inertGasConsumptionLFO.ToString();
                fooDict["inertGasConsumptionMGO"] = foo.inertGasConsumptionMGO.ToString();
                fooDict["inertGasConsumptionMDO"] = foo.inertGasConsumptionMDO.ToString();
                fooDict["hforob"] = foo.hforob.ToString();
                fooDict["lforob"] = foo.lforob.ToString();
                fooDict["mgorob"] = foo.mgorob.ToString();
                fooDict["mdorob"] = foo.mdorob.ToString();

                if (foo.meConsumptionMDO == null)
                { fooDict["meConsumptionMDO"] = ""; }
                else { fooDict["meConsumptionMDO"] = foo.meConsumptionMDO.ToString(); }
                fooDict["meConsumptionMGO"] = foo.meConsumptionMGO.ToString();
                if (foo.meConsumptionHFO == null)
                { fooDict["meConsumptionHFO"] = ""; }
                else
                {
                    fooDict["meConsumptionHFO"] = foo.meConsumptionHFO.ToString();
                }


               // var laReport = _eumrvReportGenerator.GenerateLogabstract(fooDict);
               // var now = _timeProvider.GetNow();

               // var fileName = $"{now.Year}{now.Month}{now.Day}_{Guid.NewGuid()}";

               // var laName = $"{fileName}_la.csv";

               // var laSuccess = await _azureStorageClient.UploadStringToFile($"upload/{laName}", laReport);



            }

            return Ok();
        }*/

    } 

}
