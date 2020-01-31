using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MDSConnector.Utilities
{
    interface ICertificateVerifier
    {
        public X509VerificationResult verify(X509Certificate2 certificate);
    }
}
