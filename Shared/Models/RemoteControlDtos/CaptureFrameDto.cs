using Remotely.Shared.Enums;
using System;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class CaptureFrameDto : BaseDto
    {
        [DataMember(Name = "DtoType")]
        public new BaseDtoType DtoType { get; } = BaseDtoType.CaptureFrame;

        [DataMember(Name = "EndOfFrame")]
        public bool EndOfFrame { get; set; }

        [DataMember(Name = "Height")]
        public int Height { get; set; }

        [DataMember(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember(Name = "ImageBytes")]
        public byte[] ImageBytes { get; set; }

        [DataMember(Name = "Left")]
        public int Left { get; set; }
        [DataMember(Name = "Top")]
        public int Top { get; set; }
        [DataMember(Name = "Width")]
        public int Width { get; set; }
    }
}
