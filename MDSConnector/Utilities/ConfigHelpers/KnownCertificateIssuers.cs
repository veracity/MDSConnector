using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities.ConfigHelpers
{
    //<summary>
    //Data class for the certificate authentication process.
    //Only accept client certificates that are issued by the issuers defined here.
    // (Proof of concept perposes)
    //</summary>
    public class KnownCertificateIssuers
    {
        public string[] ValidIssuers { get; set; }
    }
}
