using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class MouseDownDto : BaseDto
    {
        [DataMember(Name = "Button")]
        public int Button { get; set; }

        [DataMember(Name = "DtoType")]
        public new BinaryDtoType DtoType { get; } = BinaryDtoType.MouseDown;

        [DataMember(Name = "PercentX")]
        public double PercentX { get; set; }

        [DataMember(Name = "PercentY")]
        public double PercentY { get; set; }
    }
}
