using System.Runtime.Serialization;

namespace Immense.RemoteControl.Shared.Models.Dtos;

[DataContract]
public class KeyDownDto
{
    [DataMember(Name = "Key")]
    public string Key { get; set; } = string.Empty;
}
