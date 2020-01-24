using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector
{
    public class CertificateVerifyer
    {

        public static Dictionary<string, string> verify(X509Certificate2 certificate)
        {

            var response = new Dictionary<string, string>();
            response.Add("Valid", "True");
            response.Add("Message", "Feature not implemented on server");
            return response;
        }



    }
}
