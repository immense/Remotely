using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models
{
    public class Alert
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        [JsonIgnore]
        public Device Device { get; set; }
        public string DeviceID { get; set; }
        public string Message { get; set; }

        [JsonIgnore]
        public Organization Organization { get; set; }

        public string OrganizationID { get; set; }

        [JsonIgnore]
        public RemotelyUser User { get; set; }
        public string UserID { get; set; }
    }
}
