using Remotely.Shared.Enums;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{

    [DataContract]
    public class FileDto : BaseDto
    {
        [DataMember(Name = "Buffer")]
        public byte[] Buffer { get; set; }

        [DataMember(Name = "FileName")]
        public string FileName { get; set; }

        [DataMember(Name = "MessageId")]
        public string MessageId { get; set; }

        [DataMember(Name = "EndOfFile")]
        public bool EndOfFile { get; set; }

        [DataMember(Name = "StartOfFile")]
        public bool StartOfFile { get; set; }

        [DataMember(Name = "DtoType")]
        public new BaseDtoType DtoType { get; } = BaseDtoType.File;
    }
}
