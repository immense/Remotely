using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class MachineNameDto : IDynamicDto
    {
        public MachineNameDto(string machineName)
        {
            MachineName = machineName;
        }

        [DataMember(Name = "DtoType")]
        public DynamicDtoType DtoType { get; } = DynamicDtoType.MachineName;

        [DataMember(Name = "MachineName")]
        public string MachineName { get; }
    }
}
