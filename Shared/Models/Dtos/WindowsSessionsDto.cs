using System.Runtime.Serialization;

namespace Immense.RemoteControl.Shared.Models.Dtos;

[DataContract]
public class WindowsSessionsDto
{
    public WindowsSessionsDto(List<WindowsSession> windowsSessions)
    {
        WindowsSessions = windowsSessions;
    }


    [DataMember(Name = "WindowsSessions")]
    public List<WindowsSession> WindowsSessions { get; set; }
}
