using Remotely.Shared.Enums;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class WindowsSessionsDto : BaseDto
    {
        public WindowsSessionsDto(List<WindowsSession> windowsSessions)
        {
            WindowsSessions = windowsSessions;
        }


        [DataMember(Name = "WindowsSessions")]
        public List<WindowsSession> WindowsSessions { get; set; }


        [DataMember(Name = "DtoType")]
        public new BaseDtoType DtoType { get; } = BaseDtoType.WindowsSessions;
    }
}
