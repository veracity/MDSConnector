using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MDSConnector.Authentication
{
    public class CustomCertificateAuthenticator : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        public CustomCertificateAuthenticator(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
            )
        : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var clientCertificate = await Context.Connection.GetClientCertificateAsync();
            if (clientCertificate == null)
            {
                var defaultClaims = new[] {
                            new Claim(ClaimTypes.NameIdentifier, "Default"),
                            new Claim(ClaimTypes.Name, "Default Name"),
                        };
                var defaultIdentity = new ClaimsIdentity(defaultClaims, Scheme.Name);
                var defaultPrincipal = new ClaimsPrincipal(defaultIdentity);
                var defaultTicket = new AuthenticationTicket(defaultPrincipal, Scheme.Name);
                return AuthenticateResult.Success(defaultTicket);
            }

            if (!clientCertificate.Verify())
            {
                return AuthenticateResult.Fail("Certificate not valid");
            }

            
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
