using Immense.RemoteControl.Server.Models;
using System;

namespace Remotely.Server.Models
{
    public class RemoteControlSessionEx : RemoteControlSession
    {
        public string ServiceConnectionId { get; set; } = string.Empty;
        public string UserConnectionId { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public bool ViewOnly { get; set; }
        public string OrganizationId { get; set; } = string.Empty;
    }
}
