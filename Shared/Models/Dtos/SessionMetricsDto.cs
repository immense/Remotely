using MessagePack;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.Dtos;

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
