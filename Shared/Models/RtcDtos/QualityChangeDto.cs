using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class QualityChangeDto : BinaryDtoBase
    {
        [DataMember(Name = "QualityLevel")]
        public int QualityLevel { get; set; }

        [DataMember(Name = "DtoType")]
        public new BinaryDtoType DtoType { get; } = BinaryDtoType.QualityChange;
    }
}
