using Remotely.Shared.Enums;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class QualityChangeDto : BaseDto
    {
        [DataMember(Name = "QualityLevel")]
        public int QualityLevel { get; set; }

        [DataMember(Name = "DtoType")]
        public new BaseDtoType DtoType { get; } = BaseDtoType.QualityChange;
    }
}
