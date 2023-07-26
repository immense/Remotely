using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Text.Json.Serialization;

namespace Remotely.Shared.DtoEntityBases;

public abstract class ScriptResultBase
{
    public virtual required string DeviceID { get; set; }

    public virtual string[]? ErrorOutput { get; set; }

    public virtual bool HadErrors { get; set; }

    public virtual ScriptInputType InputType { get; set; }

    [JsonConverter(typeof(TimeSpanJsonConverter))]
    public virtual TimeSpan RunTime { get; set; }

    public virtual string ScriptInput { get; set; } = string.Empty;

    public virtual int? ScheduleId { get; set; }
    public virtual Guid? SavedScriptId { get; set; }

    public virtual int? ScriptRunId { get; set; }


    public virtual string? SenderConnectionID { get; set; }
    public virtual string? SenderUserName { get; set; } = null!;
    public virtual ScriptingShell Shell { get; set; }
    public virtual string[]? StandardOutput { get; set; }

    public virtual DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;

}
