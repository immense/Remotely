using Remotely.Shared.Enums;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class AutoQualityAdjustDto : BaseDto
    {
        [DataMember(Name = "IsOn")]
        public bool IsOn { get; set; }

        [DataMember(Name = "DtoType")]
        public new BaseDtoType DtoType { get; } = BaseDtoType.AutoQualityAdjust;
    }
}
