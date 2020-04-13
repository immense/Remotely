using System.Runtime.Serialization;

namespace Remotely.Shared.Enums
{
    [DataContract]
    public enum DynamicDtoType
    {
        [EnumMember(Value = "CaptureFrame")]
        CaptureFrame = 0,
        [EnumMember(Value = "ScreenData")]
        ScreenData = 1,
        [EnumMember(Value = "ScreenSize")]
        ScreenSize = 2,
        [EnumMember(Value = "MachineName")]
        MachineName = 3
    }
}
