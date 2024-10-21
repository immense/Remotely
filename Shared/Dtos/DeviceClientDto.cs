using Remotely.Shared.Models;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Remotely.Shared.Dtos;

[DataContract]
public class DeviceClientDto
{
    [DataMember]
    public string AgentVersion { get; set; } = string.Empty;

    [DataMember]
    public double CpuUtilization { get; set; }

    [DataMember]
    public string CurrentUser { get; set; } = string.Empty;

    [DataMember]
    public string DeviceName { get; set; } = string.Empty;

    [DataMember]
    public List<Drive> Drives { get; set; } = new();

    [DataMember]
    public string ID { get; set; } = string.Empty;

    [DataMember]
    public bool Is64Bit { get; set; }

    [DataMember]
    public bool IsOnline { get; set; }

    [DataMember]
    public string[] MacAddresses { get; set; } = Array.Empty<string>();

    [DataMember]
    public string OrganizationID { get; set; } = string.Empty;

    [DataMember]
    public Architecture OSArchitecture { get; set; }

    [DataMember]
    public string OSDescription { get; set; } = string.Empty;

    [DataMember]
    public string Platform { get; set; } = string.Empty;

    [DataMember]
    public int ProcessorCount { get; set; }

    [DataMember]
    public string PublicIP { get; set; } = string.Empty;

    [DataMember]
    public double TotalMemory { get; set; }

    [DataMember]
    public double TotalStorage { get; set; }

    [DataMember]
    public double UsedMemory { get; set; }

    [DataMember]
    public double UsedStorage { get; set; }
}
