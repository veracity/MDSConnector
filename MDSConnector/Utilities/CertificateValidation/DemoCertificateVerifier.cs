using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities
{
    public class DemoCertificateVerifier : ICertificateVerifier
    {

        public X509VerificationResult verify(X509Certificate2 certificate)
        {
            var response = new X509VerificationResult(true, "Not implemented");
            return response;
        }



    }

    
}
