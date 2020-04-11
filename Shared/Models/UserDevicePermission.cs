using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models
{
    public class UserDevicePermission
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public string UserID { get; set; }

        [JsonIgnore]
        public RemotelyUser User { get; set; }

        public string DeviceGroupID { get; set; }

        [JsonIgnore]
        public DeviceGroup DeviceGroup { get; set; }
    }
}
