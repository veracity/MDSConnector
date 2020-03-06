using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Authentication
{
    public class CertificateClaimTypes
    {

        internal const string ClaimTypeNamespace = "http://schema.mds.com";

        public const string Subject = ClaimTypeNamespace  + "/subject";
        public const string Issuer = ClaimTypeNamespace + "/Issuer";
        public const string Thumbprint = ClaimTypeNamespace + "/Thumbprint";
        public const string Admin = ClaimTypeNamespace + "/Admin";
        public const string Company = ClaimTypeNamespace + "/Company";
    }
}
