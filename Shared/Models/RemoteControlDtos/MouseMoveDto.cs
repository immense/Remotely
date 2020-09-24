using Remotely.Shared.Enums;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class MouseMoveDto : BaseDto
    {

        [DataMember(Name = "DtoType")]
        public new BaseDtoType DtoType { get; set; } = BaseDtoType.MouseMove;

        [DataMember(Name = "PercentX")]
        public double PercentX { get; set; }

        [DataMember(Name = "PercentY")]
        public double PercentY { get; set; }
    }
}
