using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities.Time
{
    /// <summary>
    /// A simple Time provider that wrappes around DateTime.now, used in certificate authentication handler to check if the certificate time is valid.
    /// Purpose is to enable easy unit testing (DateTime.now is global and static thus cannot be mocked.)
    /// A more sophisticated time provider can also be implemented if needed, following the ITimeProvider interface.
    /// </summary>
    public class DefaultTimeProvider : ITimeProvider
    {
        public DateTime GetNow()
        {
            return DateTime.Now;
        }

    }
}
