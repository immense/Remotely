using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Remotely.Shared.Models
{
    public class UserDevicePermission
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public string UserID { get; set; }
        public RemotelyUser User { get; set; }

        public string DeviceGroupID { get; set; }
        public DeviceGroup DeviceGroup { get; set; }
    }
}
