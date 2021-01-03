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
        public string AgentVersion { get; set; }
        public ICollection<Alert> Alerts { get; set; }
        [StringLength(100)]
        public string Alias { get; set; }
        public double CpuUtilization { get; set; }
        public string CurrentUser { get; set; }
        public DeviceGroup DeviceGroup { get; set; }
        public string DeviceGroupID { get; set; }
        public string DeviceName { get; set; }
        public List<Drive> Drives { get; set; }

        [Key]
        public string ID { get; set; }

        public bool Is64Bit { get; set; }
        public bool IsOnline { get; set; }
        public DateTimeOffset LastOnline { get; set; }
        public string Notes { get; set; }
        [JsonIgnore]
        public Organization Organization { get; set; }

        public string OrganizationID { get; set; }
        public Architecture OSArchitecture { get; set; }
        public string OSDescription { get; set; }
        public string Platform { get; set; }
        public int ProcessorCount { get; set; }
        public string PublicIP { get; set; }
        public string ServerVerificationToken { get; set; }
        [StringLength(200)]
        public string Tags { get; set; } = "";

        public double TotalMemory { get; set; }
        public double TotalStorage { get; set; }
        public double UsedMemory { get; set; }

        public double UsedStorage { get; set; }
        public WebRtcSetting WebRtcSetting { get; set; }
    }
}