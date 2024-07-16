using System.Runtime.Serialization;

namespace Immense.RemoteControl.Shared.Models.Dtos;

[DataContract]
public class CursorChangeDto
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

    [DataMember(Name = "HotSpotX")]
    public int HotSpotX { get; }

    [DataMember(Name = "HotSpotY")]
    public int HotSpotY { get; }

    [DataMember(Name = "ImageBytes")]
    public byte[] ImageBytes { get; }
}
