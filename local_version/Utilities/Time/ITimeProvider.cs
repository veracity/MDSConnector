using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities
{
    /// <summary>
    /// Interface definition for the timeprovider.
    /// </summary>
    public interface ITimeProvider
    {
        public DateTime GetNow();
    }
}
