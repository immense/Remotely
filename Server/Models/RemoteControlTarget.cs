using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Models
{
    public class RemoteControlTarget
    {
        public string ServiceConnectionId { get; init; }
        public bool ViewOnlyMode { get; init; }
    }
}
