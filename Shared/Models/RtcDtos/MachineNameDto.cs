using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class MachineNameDto : BinaryDtoBase
    {
        public MachineNameDto(string machineName)
        {
            MachineName = machineName;
        }

        [DataMember(Name = "DtoType")]
        public new BinaryDtoType DtoType { get; } = BinaryDtoType.MachineName;

        [DataMember(Name = "MachineName")]
        public string MachineName { get; }
    }
}
