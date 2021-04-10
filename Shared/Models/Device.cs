using Remotely.Shared.Attributes;
using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models
{
    public class Device
    {
        [Sortable]
        [Display(Name = "Agent Version")]
        public string AgentVersion { get; set; }

        public ICollection<Alert> Alerts { get; set; }
        [StringLength(100)]

        [Sortable]
        [Display(Name = "Alias")]
        public string Alias { get; set; }

        [Sortable]
        [Display(Name = "CPU Utilization")]
        public double CpuUtilization { get; set; }

        [Sortable]
        [Display(Name = "Current User")]
        public string CurrentUser { get; set; }

        public DeviceGroup DeviceGroup { get; set; }
        public string DeviceGroupID { get; set; }

        [Sortable]
        [Display(Name = "Device Name")]
        public string DeviceName { get; set; }
        public List<Drive> Drives { get; set; }

        [Key]
        public string ID { get; set; }

        public bool Is64Bit { get; set; }
        public bool IsOnline { get; set; }

        [Sortable]
        [Display(Name = "Last Online")]
        public DateTimeOffset LastOnline { get; set; }

        [StringLength(5000)]
        public string Notes { get; set; }       

        [JsonIgnore]
        public Organization Organization { get; set; }

        public string OrganizationID { get; set; }
        public Architecture OSArchitecture { get; set; }

        [Sortable]
        [Display(Name = "OS Description")]
        public string OSDescription { get; set; }

        [Sortable]
        [Display(Name = "Platform")]
        public string Platform { get; set; }

        [Sortable]
        [Display(Name = "Processor Count")]
        public int ProcessorCount { get; set; }

        public string PublicIP { get; set; }
        public string ServerVerificationToken { get; set; }

        [JsonIgnore]
        public List<ScriptResult> ScriptResults { get; set; }

        [JsonIgnore]
        public List<ScriptRun> ScriptRuns { get; set; }
        [JsonIgnore]
        public List<ScriptRun> ScriptRunsCompleted { get; set; }

        [JsonIgnore]
        public List<ScriptSchedule> ScriptSchedules { get; set; }

        [StringLength(200)]
        [Sortable]
        [Display(Name = "Tags")]
        public string Tags { get; set; } = "";

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
        public double UsedMemoryPercent => UsedMemory / TotalMemory;

        [Sortable]
        [Display(Name = "Storage Used")]
        public double UsedStorage { get; set; }

        [Sortable]
        [Display(Name = "Storage Used %")]
        public double UsedStoragePercent => UsedStorage / TotalStorage;

        public WebRtcSetting WebRtcSetting { get; set; }
    }
}