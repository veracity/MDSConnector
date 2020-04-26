using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities.ConfigHelpers
{
    //<summary>
    //Data class for the certificate authentication process.
    //Only give "admin" claim to clients that have certificate thumbprint contained here.
    // (Proof of concept perposes)
    //</summary>
    public class AdminThumbprints
    {
        public string[] Value { get; set; }
    }
}
