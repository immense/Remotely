using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.Shared.Models
{
    public class DevicePermissionLink
    {
        public string DeviceID { get; set; }
        public Device Device { get; set; }
        public string PermissionGroupID { get; set; }
        public PermissionGroup PermissionGroup { get; set; }
    }
}
