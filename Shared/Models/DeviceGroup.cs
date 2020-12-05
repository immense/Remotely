using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models
{
    public class DeviceGroup
    {
        [StringLength(200)]
        public string Name { get; set; }

        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [JsonIgnore]
        public List<Device> Devices { get; set; }

        [JsonIgnore]
        public Organization Organization { get; set; }

        public string OrganizationID { get; set; }

        [JsonIgnore]
        public List<RemotelyUser> Users { get; set; }
    }
}
