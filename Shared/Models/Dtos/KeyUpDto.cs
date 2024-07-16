using System.Runtime.Serialization;

namespace Immense.RemoteControl.Shared.Models.Dtos;

[DataContract]
public class KeyUpDto
{
    [DataMember(Name = "Key")]
    public string Key { get; set; } = string.Empty;

}
