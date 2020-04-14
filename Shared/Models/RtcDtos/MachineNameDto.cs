using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class MachineNameDto : IBinaryDto
    {
        public MachineNameDto(string machineName)
        {
            MachineName = machineName;
        }

        [DataMember(Name = "DtoType")]
        public BinaryDtoType DtoType { get; } = BinaryDtoType.MachineName;

        [DataMember(Name = "MachineName")]
        public string MachineName { get; }
    }
}
