using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDSConnector.Utilities
{
    public interface ITimeProvider
    {
        public DateTime GetNow();
    }
}
