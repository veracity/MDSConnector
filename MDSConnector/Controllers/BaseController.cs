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
        private readonly IVeracityClient _veracityClient;

        public BaseController(ILogger<BaseController> logger, IMDSClient mdsClient, IVeracityClient veracityClient)
        {
            _logger = logger;
            _mdsClient = mdsClient;
            _veracityClient = veracityClient;
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
            List<VesselNameModel> vesselNames;
            try
            {
                vesselNames = await _mdsClient.GetVesselNames();
            }
            catch (Exception e){return StatusCode(500, e.Message);}

            string res = "";
            foreach (var vesselName in vesselNames)
            {
                res += $"{vesselName.Name} {vesselName.Imo}\n";
            }

            return Ok(res);
        }

        [HttpGet]
        //[Authorize]
        [Route("infrastructure")]
        public async Task<IActionResult> GetInfrastructure()
        {

            var infrastructureRes = await _mdsClient.GetInfrastructure();

            return Ok(infrastructureRes);
        }
    }
}
