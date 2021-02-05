using MDSConnector.Utilities;
using MDSConnector.Utilities.ConfigHelpers;
using MDSConnector.Utilities.Time;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MDSConnector.Authentication
{
    //<summary>
    //A custom authentication handler that replaces the default .net core certificate authentication handler.
    //This is where you should put custom logic in order to do custom authentication based on X509Certificates
    //</summary>
    public class CustomCertificateAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        private KnownCertificateIssuers _knownCertificateIssuers;
        private ITimeProvider _timeProvider;
        private AdminThumbprints _adminThumbprints;

        public CustomCertificateAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IOptions<KnownCertificateIssuers> knownCertificateIssuers,
            IOptions<AdminThumbprints> adminThumbprints,
            ITimeProvider timeProvider
            )
        : base(options, logger, encoder, clock)
        {
            _knownCertificateIssuers = knownCertificateIssuers.Value;
            _timeProvider = timeProvider;
            _adminThumbprints = adminThumbprints.Value;

        }

        //<summary>
        //Handles the authentication for the client request. Contains logic that determines which claims the client
        //is granted
        //(Proof of concept purposes)
        //<summary>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var clientCertificate = await Context.Connection.GetClientCertificateAsync();
            if (clientCertificate == null)
            {
                var defaultClaims = new[] {
                            new Claim(ClaimTypes.NameIdentifier, "Admin"),
                        };
                var defaultIdentity = new ClaimsIdentity(defaultClaims, Scheme.Name);
                var defaultPrincipal = new ClaimsPrincipal(defaultIdentity);
                var defaultTicket = new AuthenticationTicket(defaultPrincipal, Scheme.Name);
                return AuthenticateResult.Success(defaultTicket);
            }

            //Since we are testing with self signed certificates, this would never return true.
            //This should be done in a production environment
            //if (!clientCertificate.Verify())
            //{
            //    return AuthenticateResult.Fail("Certificate not valid");
            //}

            if (!VerifyStartAndExpiration(clientCertificate))
            {
                return AuthenticateResult.Fail("Invalid notBefore and/or notAfter");
            }

            if (!VerifyIssuerDomain(clientCertificate))
            {
                return AuthenticateResult.Fail("Invalid issuer");
            }
            
            if (!VerifyIssuerAndSubject(clientCertificate))
            {
                return AuthenticateResult.Fail("Issuer and subject domain miss match");
            }



            var claims = new Claim[5];
            claims[0] = new Claim(ClaimTypes.AuthenticationMethod, "Certificate");
            claims[1] = new Claim(CertificateClaimTypes.Subject, clientCertificate.Subject);
            claims[2] = new Claim(CertificateClaimTypes.Issuer, clientCertificate.Issuer);
            claims[3] = new Claim(CertificateClaimTypes.Thumbprint, clientCertificate.Thumbprint);
            if (VerifyIsAdmin(clientCertificate))
            {
                claims[4] = new Claim(ClaimTypes.Role, "Admin");
            }
            else {
                claims[4] = new Claim(ClaimTypes.Role, "User");
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }


        //<summary>
        //Helper function for verifying a client certificate. Verifies that 
        //(Proof of concept purposes)
        //<summary>
        private bool VerifyIsAdmin(X509Certificate2 clientCerticicate)
        {
            var clientThumbprint = clientCerticicate.Thumbprint;
            return Array.Exists(_adminThumbprints.Value, t => t == clientThumbprint);
        }

        //<summary>
        //Helper function for verifying a client certificate.
        //(Proof of concept purposes)
        //<summary>
        private bool VerifyStartAndExpiration(X509Certificate2 clientCertificate)
        {
            var now = _timeProvider.GetNow();
            var notAfter = clientCertificate.NotAfter;
            var notBefore = clientCertificate.NotBefore;
            if (DateTime.Compare(notAfter, now.AddMinutes(30)) < 0)
            {
                return false;
            }

            if (DateTime.Compare(notBefore, now) > 0)
            {
                return false;
            }
            return true;
        }

        //<summary>
        //Helper function for verifying a client certificate.
        //(Proof of concept purposes)
        //<summary>
        private bool VerifyIssuerAndSubject(X509Certificate2 clientCerficate)
        {
            var issuer = clientCerficate.Issuer;
            var subject = clientCerficate.Subject;

            return issuer == subject;
        }

        //<summary>
        //Helper function for verifying a client certificate.
        //(Proof of concept purposes)
        //<summary>
        private bool VerifyIssuerDomain(X509Certificate2 clientCertificate)
        {
            var issuer = clientCertificate.Issuer;
            return Array.Exists(_knownCertificateIssuers.ValidIssuers, x => x.ToString().ToLower() == issuer.ToLower());
            //return _knownCertificateIssuers.validIssuers.Exists(x => x == issuerDomain.ToLower());

        }


    }
}
