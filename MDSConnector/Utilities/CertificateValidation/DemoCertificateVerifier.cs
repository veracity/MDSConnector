using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities
{
    public class DemoCertificateVerifier : ICertificateVerifier
    {   
        public X509VerificationResult verify(X509Certificate2 certificate, HashSet<string> trustedIssuers)
        {
            if (certificate == null)
            {
                return new X509VerificationResult(false, "No certificate provided");
            }

            var issuer = certificate.Issuer;
            var subject = certificate.Subject;
            if (issuer != subject)
            {
                return new X509VerificationResult(false, "Issuer and subject miss match");
            }

            if (!trustedIssuers.Contains(issuer))
            {
                return new X509VerificationResult(false, $"{issuer} is not a trusted certificate issuer");
            }

            var response = new X509VerificationResult(true, "Not implemented");
            return response;
        }



    }

    
}
