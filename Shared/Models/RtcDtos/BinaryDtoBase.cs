using Remotely.Shared.Enums;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class BinaryDtoBase
    {
        [DataMember(Name = "DtoType")]
        public BinaryDtoType DtoType { get; set; }
    }
}
