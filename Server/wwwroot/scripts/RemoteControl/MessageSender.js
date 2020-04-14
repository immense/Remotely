import { MainRc } from "./Main.js";
import { CtrlAltDelDto, KeyDownDto, KeyPressDto, KeyUpDto, MouseDownDto, MouseMoveDto, MouseUpDto, MouseWheelDto, QualityChangeDto, SelectScreenDto, TapDto, AutoQualityAdjustDto, ToggleAudioDto, ToggleBlockInputDto, ClipboardTransferDto } from "./RtcDtos.js";
export class MessageSender {
    SendSelectScreen(displayName) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new SelectScreenDto(displayName)), () => MainRc.RCBrowserSockets.SendSelectScreen(displayName));
    }
    SendMouseMove(percentX, percentY) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new MouseMoveDto(percentX, percentY)), () => MainRc.RCBrowserSockets.SendMouseMove(percentX, percentY));
    }
    SendMouseDown(button, percentX, percentY) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new MouseDownDto(button, percentX, percentY)), () => MainRc.RCBrowserSockets.SendMouseDown(button, percentX, percentY));
    }
    SendMouseUp(button, percentX, percentY) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new MouseUpDto(button, percentX, percentY)), () => MainRc.RCBrowserSockets.SendMouseUp(button, percentX, percentY));
    }
    SendTap(percentX, percentY) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new TapDto(percentX, percentY)), () => MainRc.RCBrowserSockets.SendTap(percentX, percentY));
    }
    SendMouseWheel(deltaX, deltaY) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new MouseWheelDto(deltaX, deltaY)), () => MainRc.RCBrowserSockets.SendMouseWheel(deltaX, deltaY));
    }
    SendKeyDown(key) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new KeyDownDto(key)), () => MainRc.RCBrowserSockets.SendKeyDown(key));
    }
    SendKeyUp(key) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new KeyUpDto(key)), () => MainRc.RCBrowserSockets.SendKeyUp(key));
    }
    SendKeyPress(key) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new KeyPressDto(key)), () => MainRc.RCBrowserSockets.SendKeyPress(key));
    }
    SendCtrlAltDel() {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new CtrlAltDelDto()), () => MainRc.RCBrowserSockets.SendCtrlAltDel());
    }
    SendQualityChange(qualityLevel) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new QualityChangeDto(qualityLevel)), () => MainRc.RCBrowserSockets.SendQualityChange(qualityLevel));
    }
    SendAutoQualityAdjust(isOn) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new AutoQualityAdjustDto(isOn)), () => MainRc.RCBrowserSockets.SendAutoQualityAdjust(isOn));
    }
    SendToggleAudio(toggleOn) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new ToggleAudioDto(toggleOn)), () => MainRc.RCBrowserSockets.SendToggleAudio(toggleOn));
    }
    ;
    SendToggleBlockInput(toggleOn) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new ToggleBlockInputDto(toggleOn)), () => MainRc.RCBrowserSockets.SendToggleBlockInput(toggleOn));
    }
    SendClipboardTransfer(text, typeText) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new ClipboardTransferDto(text, typeText)), () => MainRc.RCBrowserSockets.SendClipboardTransfer(text, typeText));
    }
    SendToAgent(rtcSend, websocketSend) {
        if (MainRc.RtcSession.DataChannel && MainRc.RtcSession.DataChannel.readyState == "open") {
            rtcSend();
        }
        else {
            websocketSend();
        }
    }
}
//# sourceMappingURL=MessageSender.js.map