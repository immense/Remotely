using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models
{
    public class DeviceGroup
    {
        [StringLength(200)]
        public string Name { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }

        [JsonIgnore]
        public List<Device> Devices { get; set; }

        [JsonIgnore]
        public Organization Organization { get; set; }

        public string OrganizationID { get; set; }

        [JsonIgnore]
        public List<RemotelyUser> Users { get; set; }

        [JsonIgnore]
        public List<ScriptSchedule> ScriptSchedules { get; set; }
    }
}
