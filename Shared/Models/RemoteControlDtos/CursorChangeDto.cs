using Remotely.Shared.Enums;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class CursorChangeDto : BaseDto
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
        public new BaseDtoType DtoType { get; } = BaseDtoType.CursorChange;

        [DataMember(Name = "HotSpotX")]
        public int HotSpotX { get; }

        [DataMember(Name = "HotSpotY")]
        public int HotSpotY { get; }

        [DataMember(Name = "ImageBytes")]
        public byte[] ImageBytes { get; }
    }
}
