using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities
{
    public struct X509VerificationResult
    {

        public bool valid;
        public string reason;

        public X509VerificationResult(bool validity, string r)
        {
            valid = validity;
            reason = r;
        }

    }
}
