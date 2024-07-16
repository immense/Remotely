using System.Runtime.Serialization;

namespace Remotely.Shared.Models.Dtos;

[DataContract]
public enum DtoType
{
    [EnumMember]
    ScreenData = 1,
    [EnumMember]
    ScreenSize = 2,
    [EnumMember]
    ClipboardText = 4,
    [EnumMember]
    AudioSample = 5,
    [EnumMember]
    CursorChange = 6,
    [EnumMember]
    SelectScreen = 7,
    [EnumMember]
    MouseMove = 8,
    [EnumMember]
    MouseDown = 9,
    [EnumMember]
    MouseUp = 10,
    [EnumMember]
    Tap = 11,
    [EnumMember]
    MouseWheel = 12,
    [EnumMember]
    KeyDown = 13,
    [EnumMember]
    KeyUp = 14,
    [EnumMember]
    CtrlAltDel = 15,
    [EnumMember]
    ToggleAudio = 17,
    [EnumMember]
    ToggleBlockInput = 18,
    [EnumMember]
    TextTransfer = 19,
    [EnumMember]
    KeyPress = 20,
    [EnumMember]
    File = 22,
    [EnumMember]
    WindowsSessions = 23,
    [EnumMember]
    SetKeyStatesUp = 24,
    [EnumMember]
    FrameReceived = 25,
    [EnumMember]
    OpenFileTransferWindow = 27,
    [EnumMember]
    SessionMetrics = 28
}
