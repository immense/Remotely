using System.Runtime.Serialization;

namespace Immense.RemoteControl.Shared.Models.Dtos;

[DataContract]
public class MouseDownDto
{
    [DataMember(Name = "Button")]
    public int Button { get; set; }

    [DataMember(Name = "PercentX")]
    public double PercentX { get; set; }

    [DataMember(Name = "PercentY")]
    public double PercentY { get; set; }
}
