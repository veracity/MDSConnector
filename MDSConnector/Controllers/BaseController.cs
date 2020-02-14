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

namespace MDSConnector.Controllers
{
    [ApiController]
    [Route("")]
    public class BaseController: ControllerBase
    {
        private readonly ILogger<BaseController> _logger;
        private readonly MDSClient _mdsClient;
        private readonly VeracityClient _veracityClient;

        public BaseController(ILogger<BaseController> logger, MDSClient mdsClient, VeracityClient veracityClient)
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
        public IActionResult GetVesselNames()
        {

            var tokenRes = _mdsClient.GetVesselNames();

            return Ok(tokenRes);
        }
    }
}
