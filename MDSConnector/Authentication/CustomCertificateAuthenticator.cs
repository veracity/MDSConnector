﻿using Microsoft.AspNetCore.Authentication;
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
                        };
                var defaultIdentity = new ClaimsIdentity(defaultClaims, Scheme.Name);
                var defaultPrincipal = new ClaimsPrincipal(defaultIdentity);
                var defaultTicket = new AuthenticationTicket(defaultPrincipal, Scheme.Name);
                return AuthenticateResult.Success(defaultTicket);
            }

            //if (!clientCertificate.Verify())
            //{
            //    return AuthenticateResult.Fail("Certificate not valid");
            //}

            if (!validIssuerAndSubject(clientCertificate))
            {
                return AuthenticateResult.Fail("Issuer and subject domain miss match");
            }

            var claims = new Claim[5];
            claims[0] = new Claim(ClaimTypes.AuthenticationMethod, "Certificate");
            claims[1] = new Claim(CertificateClaimTypes.Subject, clientCertificate.Subject);
            claims[2] = new Claim(CertificateClaimTypes.Issuer, clientCertificate.Issuer);
            claims[3] = new Claim(CertificateClaimTypes.Thumbprint, clientCertificate.Thumbprint);
            if (clientCertificate.Thumbprint == "8FE88E11E00CF7C0C4E93A1E3BDF977EF2785BE3")
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

        private bool validIssuerAndSubject(X509Certificate2 clientCerficate)
        {
            var issuer = clientCerficate.Issuer;
            var subject = clientCerficate.Subject;

            var regex = new Regex(@"(?<=@)[^.]+(?=\.)");

            var issuerDomain = regex.Match(issuer).Value;
            var subjectDomain = regex.Match(subject).Value;

            return issuerDomain == subjectDomain;
        }


    }
}
