using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities.ConfigHelpers
{
    public class VeracityConfig
    {
        public string baseUrl { get; set; }
        public string subscriptionKey { get; set; }

        public override string ToString()
        {
            return $"baseUrl: {baseUrl} \n subscriptionKey: {subscriptionKey}";
        }
    }
}
