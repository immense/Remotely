using System.Runtime.Serialization;

namespace Remotely.Shared.Models.Dtos;

[DataContract]
public class AudioSampleDto
{
    public AudioSampleDto(byte[] buffer)
    {
        Buffer = buffer;
    }

    [DataMember(Name = "Buffer")]
    public byte[] Buffer { get; }
}
