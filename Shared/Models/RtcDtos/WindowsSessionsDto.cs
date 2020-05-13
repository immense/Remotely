using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class WindowsSessionsDto : BinaryDtoBase
    {
        public WindowsSessionsDto(List<WindowsSession> windowsSessions)
        {
            WindowsSessions = windowsSessions;
        }


        [DataMember(Name = "WindowsSessions")]
        public List<WindowsSession> WindowsSessions { get; set; }


        [DataMember(Name = "DtoType")]
        public new BinaryDtoType DtoType { get; } = BinaryDtoType.WindowsSessions;
    }
}
