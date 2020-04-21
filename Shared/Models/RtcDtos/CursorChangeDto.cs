using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class CursorChangeDto : BinaryDtoBase
    {
        public CursorChangeDto(byte[] imageBytes, int hotSpotX, int hotSpotY, string cssOverride)
        {
            ImageBytes = imageBytes;
            HotSpotX = hotSpotX;
            HotSpotY = hotSpotY;
            CssOverride = cssOverride;
        }

        [DataMember(Name = "CssOverride")]
        public string CssOverride { get; }

        [DataMember(Name = "DtoType")]
        public new BinaryDtoType DtoType { get; } = BinaryDtoType.CursorChange;

        [DataMember(Name = "HotSpotX")]
        public int HotSpotX { get; }

        [DataMember(Name = "HotSpotY")]
        public int HotSpotY { get; }

        [DataMember(Name = "ImageBytes")]
        public byte[] ImageBytes { get; }
    }
}
