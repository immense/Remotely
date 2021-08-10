using Remotely.Shared.Enums;
using System;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class CaptureFrameDto : BaseDto
    {
        [DataMember(Name = "DtoType")]
        public override BaseDtoType DtoType { get; init; } = BaseDtoType.CaptureFrame;

        [DataMember(Name = "EndOfFrame")]
        public bool EndOfFrame { get; init; }

        [DataMember(Name = "Height")]
        public int Height { get; init; }

        [DataMember(Name = "ImageBytes")]
        public byte[] ImageBytes { get; init; }

        [DataMember(Name = "Left")]
        public int Left { get; init; }
        [DataMember(Name = "Top")]
        public int Top { get; init; }
        [DataMember(Name = "Width")]
        public int Width { get; init; }

        [DataMember(Name = "Sequence")]
        public long Sequence { get; init; }
    }
}
