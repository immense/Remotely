using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Immense.RemoteControl.Shared.Models.Dtos;

[DataContract]
public class SessionMetricsDto
{
    [SerializationConstructor]
    public SessionMetricsDto(double mbps, double fps, double roundTripLatency, bool isGpuAccelerated)
    {
        Mbps = mbps;
        Fps = fps;
        RoundTripLatency = roundTripLatency;
        IsGpuAccelerated = isGpuAccelerated;
    }

    [DataMember]
    public double Mbps { get; }

    [DataMember]
    public double Fps { get; }

    [DataMember]
    public double RoundTripLatency { get; }

    [DataMember]
    public bool IsGpuAccelerated { get; }
}
