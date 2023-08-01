using Immense.RemoteControl.Shared.Models;
using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Remotely.Shared.Entities;

public class Organization
{
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    public ICollection<ApiToken> ApiTokens { get; set; } = new List<ApiToken>();

    public BrandingInfo? BrandingInfo { get; set; }
    public string? BrandingInfoId { get; set; }

    public ICollection<ScriptResult> ScriptResults { get; set; } = new List<ScriptResult>();

    public ICollection<ScriptRun> ScriptRuns { get; set; } = new List<ScriptRun>();
    public ICollection<SavedScript> SavedScripts { get; set; } = new List<SavedScript>();

    public ICollection<ScriptSchedule> ScriptSchedules { get; set; } = new List<ScriptSchedule>();

    public ICollection<DeviceGroup> DeviceGroups { get; set; } = new List<DeviceGroup>();

    public ICollection<Device> Devices { get; set; } = new List<Device>();

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ID { get; set; } = null!;

    public ICollection<InviteLink> InviteLinks { get; set; } = new List<InviteLink>();

    public bool IsDefaultOrganization { get; set; }

    [StringLength(25)]
    public required string OrganizationName { get; set; }

    public ICollection<RemotelyUser> RemotelyUsers { get; set; } = new List<RemotelyUser>();
    public ICollection<SharedFile> SharedFiles { get; set; } = new List<SharedFile>();
}