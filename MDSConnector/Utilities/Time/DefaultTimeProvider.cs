using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities.Time
{
    public class DefaultTimeProvider : ITimeProvider
    {
        public DateTime GetNow()
        {
            return DateTime.Now;
        }

    }
}
