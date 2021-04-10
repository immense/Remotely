using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Remotely.Shared.Models
{
    public class ScriptRun
    {
        [JsonIgnore]
        public List<Device> Devices { get; set; }

        [JsonIgnore]
        public List<Device> DevicesCompleted { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Initiator { get; set; }

        public ScriptInputType InputType { get; set; }

        [JsonIgnore]
        public Organization Organization { get; set; }

        public string OrganizationID { get; set; }
        [JsonIgnore]
        public List<ScriptResult> Results { get; set; }

        public DateTimeOffset RunAt { get; set; }
        public bool RunOnNextConnect { get; set; }
        public Guid? SavedScriptId { get; set; }
        public int? ScheduleId { get; set; }
    }
}
