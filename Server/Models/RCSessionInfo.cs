using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Models
{
    public class RCSessionInfo
    {
        public string AttendedSessionID { get; set; }
        public string DeviceID { get; set; }
        public string MachineName { get; set; }
        public RemoteControlMode Mode { get; set; }
        public string OrganizationID { get; set; }
        public string RCSocketID { get; set; }
        public string RequesterName { get; set; }
        public string ServiceID { get; set; }
        public DateTime StartTime { get; set; }
    }
}
