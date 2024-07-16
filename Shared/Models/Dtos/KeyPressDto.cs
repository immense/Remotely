using System.Runtime.Serialization;

namespace Remotely.Shared.Models.Dtos;

[DataContract]
public class KeyPressDto
{
    [DataMember(Name = "Key")]
    public string Key { get; set; } = string.Empty;
}
