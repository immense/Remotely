using System.Runtime.Serialization;

namespace Remotely.Shared.Models.Dtos;

[DataContract]
public class SelectScreenDto
{
    [DataMember(Name = "DisplayName")]
    public string DisplayName { get; set; } = string.Empty;
}
