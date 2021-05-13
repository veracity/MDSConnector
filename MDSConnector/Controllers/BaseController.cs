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


        [HttpGet]
        [Route("/ping")]
        //[CertificateAuthorized]
        public async Task<string> ping()
        {
            return await _mdsClient.PingNeuron();
        }

        /// <summary>
        /// Endpoint used for triggering EUMRV report generating feature
        /// </summary>
        [HttpGet]
        [Route("/eumrv")]
        //[CertificateAuthorized]
        public async Task<IActionResult> GenerateEUMRVReport()
        {
            var asyncRequests = new Dictionary<string, Task<string>>();
            asyncRequests["BunkerNeuron"] = _mdsClient.GetBunkerNeuron();
            asyncRequests["LANeuron"] = _mdsClient.GetLogAbstractNeuron();


            var results = new Dictionary<string, string>();
            foreach (var item in asyncRequests)
            {
                results[item.Key] = await item.Value;
            }

            var BunkerNeuron = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(results["BunkerNeuron"]);
            var LANeuron = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(results["LANeuron"]);

            var laReport = _eumrvReportGenerator.GenerateLogabstract(LANeuron);
            var bunkerReport = _eumrvReportGenerator.GenerateBunkerReport(BunkerNeuron);

            var now = _timeProvider.GetNow();

            var fileName = $"{now.Year}{now.Month}{now.Day}_{Guid.NewGuid()}";

            var laName = $"{fileName}_la.csv";
            var bunkerName = $"{fileName}_bn.csv";
            var laSuccess = await _azureStorageClient.UploadStringToFile($"upload/{laName}", laReport);
            var bunkerSuccess = await _azureStorageClient.UploadStringToFile($"upload/{bunkerName}", bunkerReport);

            if (bunkerSuccess && laSuccess)
            {
                return Ok("Reports uploaded successfully");
            }
            else
            {
                return StatusCode(500);
            }
        }
    }
}
