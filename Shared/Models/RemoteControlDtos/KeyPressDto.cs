using Remotely.Shared.Enums;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class KeyPressDto : BaseDto
    {
        [DataMember(Name = "Key")]
        public string Key { get; set; }

        [DataMember(Name = "DtoType")]
        public new BaseDtoType DtoType { get; } = BaseDtoType.KeyPress;
    }
}
