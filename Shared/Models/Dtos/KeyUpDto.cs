using System.Runtime.Serialization;

namespace Remotely.Shared.Models.Dtos;

[DataContract]
public class KeyUpDto
{
    [DataMember(Name = "Key")]
    public string Key { get; set; } = string.Empty;

}
