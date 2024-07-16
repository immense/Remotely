using System.Runtime.Serialization;

namespace Immense.RemoteControl.Shared.Models.Dtos;

[DataContract]
public class ToggleBlockInputDto
{
    [DataMember(Name = "ToggleOn")]
    public bool ToggleOn { get; set; }
}
