using Remotely.Shared.Attributes;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Entities;

public class Device
{

    [Sortable]
    [Display(Name = "Agent Version")]
    public string? AgentVersion { get; set; }

    public ICollection<Alert>? Alerts { get; set; }

    [StringLength(100)]
    [Sortable]
    [Display(Name = "Alias")]
    public string? Alias { get; set; }

    [Sortable]
    [Display(Name = "CPU Utilization")]
    public double CpuUtilization { get; set; }

    [Sortable]
    [Display(Name = "Current User")]
    public string? CurrentUser { get; set; }

    public DeviceGroup? DeviceGroup { get; set; }
    public string? DeviceGroupID { get; set; }

    [Sortable]
    [Display(Name = "Device Name")]
    public string? DeviceName { get; set; }
    public List<Drive>? Drives { get; set; }

    [Key]
    public string ID { get; set; } = Guid.NewGuid().ToString();

    public bool Is64Bit { get; set; }
    public bool IsOnline { get; set; }

    [Sortable]
    [Display(Name = "Last Online")]
    public DateTimeOffset LastOnline { get; set; }

    [Display(Name = "MAC Addresses")]
    public string[] MacAddresses { get; set; } = Array.Empty<string>();

    [StringLength(5000)]
    public string? Notes { get; set; }

    [JsonIgnore]
    public Organization? Organization { get; set; }

    public string OrganizationID { get; set; } = null!;
    public Architecture OSArchitecture { get; set; }

    [Sortable]
    [Display(Name = "OS Description")]
    public string? OSDescription { get; set; }

    [Sortable]
    [Display(Name = "Platform")]
    public string? Platform { get; set; }

    [Sortable]
    [Display(Name = "Processor Count")]
    public int ProcessorCount { get; set; }

    public string? PublicIP { get; set; }

    [JsonIgnore]
    public List<ScriptResult> ScriptResults { get; set; } = new();

    [JsonIgnore]
    public List<ScriptRun> ScriptRuns { get; set; } = new();

    [JsonIgnore]
    public List<ScriptSchedule> ScriptSchedules { get; set; } = new();

    public string? ServerVerificationToken { get; set; }

    [StringLength(200)]
    [Sortable]
    [Display(Name = "Tags")]
    public string? Tags { get; set; }

    [Sortable]
    [Display(Name = "Memory Total")]
    public double TotalMemory { get; set; }

    [Sortable]
    [Display(Name = "Storage Total")]
    public double TotalStorage { get; set; }

    [Sortable]
    [Display(Name = "Memory Used")]
    public double UsedMemory { get; set; }

    [Sortable]
    [Display(Name = "Memory Used %")]
    public double UsedMemoryPercent
    {
        get
        {
            if (TotalMemory == 0)
            {
                return 0;
            }
            return UsedMemory / TotalMemory;
        }
    }

    [Sortable]
    [Display(Name = "Storage Used")]
    public double UsedStorage { get; set; }

    [Sortable]
    [Display(Name = "Storage Used %")]
    public double UsedStoragePercent
    {
        get
        {
            if (TotalStorage == 0)
            {
                return 0;
            }
            return UsedStorage / TotalStorage;
        }
    }
}