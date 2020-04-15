using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class TapDto : BinaryDtoBase
    {

        [DataMember(Name = "DtoType")]
        public new BinaryDtoType DtoType { get; } = BinaryDtoType.Tap;

        [DataMember(Name = "PercentX")]
        public double PercentX { get; set; }

        [DataMember(Name = "PercentY")]
        public double PercentY { get; set; }
    }
}
