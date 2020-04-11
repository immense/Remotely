using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models
{
    public class ApiToken
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public DateTimeOffset? LastUsed { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [JsonIgnore]
        public Organization Organization { get; set; }

        public string OrganizationID { get; set; }
        public string Secret { get; set; }
        public string Token { get; set; }
    }
}
