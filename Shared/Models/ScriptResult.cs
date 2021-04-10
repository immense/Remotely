using Remotely.Shared.Enums;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models
{
    public class ScriptResult
    {
        [JsonIgnore]
        public Device Device { get; set; }

        public string DeviceID { get; set; }

        public string[] ErrorOutput { get; set; }

        public bool HadErrors { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }


        public ScriptInputType InputType { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public Organization Organization { get; set; }
        public string OrganizationID { get; set; }
        [JsonConverter(typeof(TimeSpanJsonConverter))]
        public TimeSpan RunTime { get; set; }

        [JsonIgnore]
        public ScriptSchedule Schedule { get; set; }

        public string ScriptInput { get; set; }

        public int? ScheduleId { get; set; }
        public Guid? SavedScriptId { get; set; }

        public int? ScriptRunId { get; set; }


        public string SenderConnectionID { get; set; }
        public string SenderUserName { get; set; }
        public ScriptingShell Shell { get; set; }
        public string[] StandardOutput { get; set; }
       
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;
    }
}
