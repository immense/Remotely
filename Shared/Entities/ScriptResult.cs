using Remotely.Shared.DtoEntityBases;
using Remotely.Shared.Enums;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Entities;

public class ScriptResult : ScriptResultBase
{
    [JsonIgnore]
    public Device? Device { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ID { get; set; } = null!;

    [JsonIgnore]
    [IgnoreDataMember]
    public Organization? Organization { get; set; }
    public required string OrganizationID { get; set; }

    [JsonIgnore]
    public SavedScript? SavedScript { get; set; }

    [JsonIgnore]
    public ScriptSchedule? Schedule { get; set; }

    [JsonIgnore]
    public ScriptRun? ScriptRun { get; set; }
}
