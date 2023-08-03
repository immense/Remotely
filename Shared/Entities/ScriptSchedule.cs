using Remotely.Shared.Enums;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Remotely.Shared.Entities;

public class ScriptSchedule
{
    public DateTimeOffset CreatedAt { get; set; }

    [JsonIgnore]
    public RemotelyUser? Creator { get; set; }

    public string CreatorId { get; set; } = null!;

    [JsonIgnore]
    public List<DeviceGroup> DeviceGroups { get; set; } = new();

    [JsonIgnore]
    public List<Device> Devices { get; set; } = new();

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public RepeatInterval Interval { get; set; }

    public DateTimeOffset? LastRun { get; set; }

    public required string Name { get; set; }

    public DateTimeOffset NextRun { get; set; }

    public DateTimeOffset StartAt { get; set; }

    [JsonIgnore]
    public Organization? Organization { get; set; }
    public string OrganizationID { get; set; } = null!;

    public bool RunOnNextConnect { get; set; } = true;
    public Guid SavedScriptId { get; set; }

    [JsonIgnore]
    public List<ScriptRun> ScriptRuns { get; set; } = new();
}
