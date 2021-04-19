using System.Runtime.Serialization;

namespace Remotely.Shared.Enums
{
    [DataContract]
    public enum BaseDtoType
    {
        [EnumMember(Value = "CaptureFrame")]
        CaptureFrame = 0,
        [EnumMember(Value = "ScreenData")]
        ScreenData = 1,
        [EnumMember(Value = "ScreenSize")]
        ScreenSize = 2,
        [EnumMember(Value = "MachineName")]
        MachineName = 3,
        [EnumMember(Value = "ClipboardText")]
        ClipboardText = 4,
        [EnumMember(Value = "AudioSample")]
        AudioSample = 5,
        [EnumMember(Value = "CursorChange")]
        CursorChange = 6,
        [EnumMember(Value = "SelectScreen")]
        SelectScreen = 7,
        [EnumMember(Value = "MouseMove")]
        MouseMove = 8,
        [EnumMember(Value = "MouseDown")]
        MouseDown = 9,
        [EnumMember(Value = "MouseUp")]
        MouseUp = 10,
        [EnumMember(Value = "Tap")]
        Tap = 11,
        [EnumMember(Value = "MouseWheel")]
        MouseWheel = 12,
        [EnumMember(Value = "KeyDown")]
        KeyDown = 13,
        [EnumMember(Value = "KeyUp")]
        KeyUp = 14,
        [EnumMember(Value = "CtrlAltDel")]
        CtrlAltDel = 15,
        [EnumMember(Value = "ToggleAudio")]
        ToggleAudio = 17,
        [EnumMember(Value = "ToggleBlockInput")]
        ToggleBlockInput = 18,
        [EnumMember(Value = "ClipboardTransfer")]
        ClipboardTransfer = 19,
        [EnumMember(Value = "KeyPress")]
        KeyPress = 20,
        [EnumMember(Value = "File")]
        File = 22,
        [EnumMember(Value = "WindowsSessions")]
        WindowsSessions = 23,
        [EnumMember(Value = "SetKeyStatesUp")]
        SetKeyStatesUp = 24,
        [EnumMember(Value = "FrameReceived")]
        FrameReceived = 25,
        [EnumMember(Value = "ToggleWebRtcVideo")]
        ToggleWebRtcVideo = 26,
        [EnumMember(Value = "OpenFileTransferWindow")]
        OpenFileTransferWindow = 27
    }
}
