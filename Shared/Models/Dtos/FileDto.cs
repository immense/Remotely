using System.Runtime.Serialization;

namespace Immense.RemoteControl.Shared.Models.Dtos;


[DataContract]
public class FileDto
{
    [DataMember(Name = "Buffer")]
    public byte[] Buffer { get; set; } = Array.Empty<byte>();

    [DataMember(Name = "FileName")]
    public string FileName { get; set; } = string.Empty;

    [DataMember(Name = "MessageId")]
    public string MessageId { get; set; } = string.Empty;

    [DataMember(Name = "EndOfFile")]
    public bool EndOfFile { get; set; }

    [DataMember(Name = "StartOfFile")]
    public bool StartOfFile { get; set; }
}
