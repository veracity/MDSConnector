using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MDSConnector.Controllers
{
    [ApiController]
    [Route("")]
    public class BaseController: ControllerBase
    {

        private readonly ILogger<BaseController> _logger;

        public BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {


            var user = HttpContext.User;
            var request = HttpContext.Request;
            var context = HttpContext;
            var connection = HttpContext.Connection;
            //if (clientCertificate == null)
            //{
            //    Response.StatusCode = 401;
            //    return "You must include a X509 certificate with your request";
            //}
            
            //return clientCertificate.ToString();
            return "Hello, this is server";
            
        }
    }
}
