using Remotely.Shared.Enums;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class MachineNameDto : BaseDto
    {
        public MachineNameDto(string machineName)
        {
            MachineName = machineName;
        }

        [DataMember(Name = "DtoType")]
        public new BaseDtoType DtoType { get; } = BaseDtoType.MachineName;

        [DataMember(Name = "MachineName")]
        public string MachineName { get; }
    }
}
