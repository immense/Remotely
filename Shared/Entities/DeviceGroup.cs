using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Entities;

public class DeviceGroup
{
    [StringLength(200)]
    public required string Name { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ID { get; set; } = null!;

    [JsonIgnore]
    public List<Device> Devices { get; set; } = new();

    [JsonIgnore]
    public Organization? Organization { get; set; }

    public string OrganizationID { get; set; } = null!;

    [JsonIgnore]
    public List<RemotelyUser> Users { get; set; } = new();

    [JsonIgnore]
    public List<ScriptSchedule>? ScriptSchedules { get; set; }
}
