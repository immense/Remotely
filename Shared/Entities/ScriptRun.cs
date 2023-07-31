using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Remotely.Shared.Entities;

public class ScriptRun
{
    [JsonIgnore]
    public List<Device>? Devices { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Initiator { get; set; }

    public ScriptInputType InputType { get; set; }

    [JsonIgnore]
    public Organization? Organization { get; set; }

    public string OrganizationID { get; set; } = null!;

    [JsonIgnore]
    public List<ScriptResult>? Results { get; set; }

    public DateTimeOffset RunAt { get; set; }
    public bool RunOnNextConnect { get; set; }

    [JsonIgnore]
    public SavedScript? SavedScript { get; set; }
    public Guid? SavedScriptId { get; set; }

    [JsonIgnore]
    public ScriptSchedule? Schedule { get; set; }
    public int? ScheduleId { get; set; }
}
