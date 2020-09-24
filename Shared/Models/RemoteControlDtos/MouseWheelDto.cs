using Remotely.Shared.Enums;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class MouseWheelDto : BaseDto
    {

        [DataMember(Name = "DtoType")]
        public new BaseDtoType DtoType { get; } = BaseDtoType.MouseWheel;

        [DataMember(Name = "DeltaX")]
        public double DeltaX { get; set; }

        [DataMember(Name = "DeltaY")]
        public double DeltaY { get; set; }
    }
}
