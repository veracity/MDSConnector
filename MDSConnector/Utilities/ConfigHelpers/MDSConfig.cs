using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities.ConfigHelpers
{
    public class MDSConfig
    {
        public string username { get; set; }
        public string password { get; set; }
        public string baseUrl { get; set; }
        
        public override string ToString()
        {
            return $"username: {username} \n password: {password} \n baseUrl {baseUrl}";
        }
    }

}
