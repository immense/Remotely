using System.Runtime.Serialization;

namespace Immense.RemoteControl.Shared.Models.Dtos;

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
