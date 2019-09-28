using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models
{
    public class EventLog
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public EventTypes EventType { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string OrganizationID { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        [JsonIgnore]
        public virtual Organization Organization { get; set; }
    }
    public enum EventTypes
    {
        Info = 0,
        Error = 1,
        Debug = 2
    }
}
