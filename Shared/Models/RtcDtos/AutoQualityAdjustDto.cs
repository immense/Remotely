using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class AutoQualityAdjustDto : BinaryDtoBase
    {
        [DataMember(Name = "IsOn")]
        public bool IsOn { get; set; }

        [DataMember(Name = "DtoType")]
        public new BinaryDtoType DtoType { get; } = BinaryDtoType.AutoQualityAdjust;
    }
}
