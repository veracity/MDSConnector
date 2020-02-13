using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MDSConnector
{
    public class CustomCertificateAuthenticator : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        //private readonly IUserServ ice _userService;

        public CustomCertificateAuthenticator(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
            //IUserService userService
            )
        : base(options, logger, encoder, clock)
        {
            //    _userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var clientCertificate = await Context.Connection.GetClientCertificateAsync();
            if (clientCertificate == null)
            {
                return AuthenticateResult.NoResult();
            }
            //if (!Request.Headers.ContainsKey("Authorization"))
            //    return AuthenticateResult.Fail("Missing Authorization Header");

            //User user = null;
            //try
            //{
            //    var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            //    var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            //    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            //    var username = credentials[0];
            //    var password = credentials[1];
            //    user = await _userService.Authenticate(username, password);
            //}
            //catch
            //{
            //    return AuthenticateResult.Fail("Invalid Authorization Header");
            //}

            //if (user == null)
            //    return AuthenticateResult.Fail("Invalid Username or Password");

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, "TEST"),
                new Claim(ClaimTypes.Name, "THIS IS A TEST"),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }


    }
}
