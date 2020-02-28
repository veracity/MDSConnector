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

namespace MDSConnector.Controllers
{
    [ApiController]
    [Route("")]
    public class BaseController: ControllerBase
    {
        private readonly ILogger<BaseController> _logger;
        private readonly IMDSClient _mdsClient;
        private readonly IAzureStorageClient _azureStorageClient;

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
            return "This is the server";
        }


        [HttpGet]
        //[Authorize]
        [Route("getvesselnames")]
        public async Task<IActionResult> GetVesselNames()
        {
            try
            {
                var vesselNames = await _mdsClient.GetVesselNames();
                var fileName = "vesselNames/vesselNames_" + Guid.NewGuid().ToString() + ".json"; 
                await _azureStorageClient.UploadVesselNames(fileName, vesselNames);
            }
            catch (Exception e){return StatusCode(500, e.Message);}

            return Ok();
        }

        [HttpGet]
        //[Authorize]
        [Route("infrastructure")]
        public async Task<IActionResult> GetInfrastructure()
        {

            try
            {
                var infrastructureRes = await _mdsClient.GetInfrastructure();
                var fileName = "infrastructure/infrastructure_" + Guid.NewGuid().ToString() + ".json";
                await _azureStorageClient.UploadStringToFile(fileName, infrastructureRes);
            }
            catch (Exception e){ return StatusCode(500, e.Message); }


            return Ok();
        }
    }
}
