using Remotely.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models
{
    public class EventLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }
        public EventType EventType { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string OrganizationID { get; set; }
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;
        [JsonIgnore]
        public Organization Organization { get; set; }
    }
}
