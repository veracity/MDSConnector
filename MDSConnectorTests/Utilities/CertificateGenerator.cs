using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MDSConnectorTests.Utilities
{
    class CertificateGenerator
    {

        public static X509Certificate2 CreateSelfSignedCertificate(string issuerName, DateTime notBefore, DateTime notAfter)
        {
            var ecdsa = ECDsa.Create(); // generate asymmetric key pair
            var req = new CertificateRequest(issuerName, ecdsa, HashAlgorithmName.SHA256);
            var cert = req.CreateSelfSigned(notBefore, notAfter);
            
            return cert;

        }
    }




}

