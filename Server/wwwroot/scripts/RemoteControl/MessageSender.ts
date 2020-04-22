import { MainRc } from "./Main.js";
import {
    CtrlAltDelDto,
    KeyDownDto,
    KeyPressDto,
    KeyUpDto,
    MouseDownDto,
    MouseMoveDto,
    MouseUpDto,
    MouseWheelDto,
    QualityChangeDto,
    SelectScreenDto,
    TapDto,
    AutoQualityAdjustDto,
    ToggleAudioDto,
    ToggleBlockInputDto,
    ClipboardTransferDto,
    FileDto
} from "./RtcDtos.js";
import { CreateGUID, When } from "../Utilities.js";
import { FileTransferProgress } from "./UI.js";

export class MessageSender {
    SendSelectScreen(displayName: string) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new SelectScreenDto(displayName)),
            () => MainRc.RCBrowserSockets.SendSelectScreen(displayName));
    }
    SendMouseMove(percentX: number, percentY: number) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new MouseMoveDto(percentX, percentY)),
            () => MainRc.RCBrowserSockets.SendMouseMove(percentX, percentY));
    }
    SendMouseDown(button: number, percentX: number, percentY: number) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new MouseDownDto(button, percentX, percentY)),
            () => MainRc.RCBrowserSockets.SendMouseDown(button, percentX, percentY));
    }
    SendMouseUp(button: number, percentX: number, percentY: number) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new MouseUpDto(button, percentX, percentY)),
            () => MainRc.RCBrowserSockets.SendMouseUp(button, percentX, percentY));
    }
    SendTap(percentX: number, percentY: number) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new TapDto(percentX, percentY)),
            () => MainRc.RCBrowserSockets.SendTap(percentX, percentY));
    }
    SendMouseWheel(deltaX: number, deltaY: number) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new MouseWheelDto(deltaX, deltaY)),
            () => MainRc.RCBrowserSockets.SendMouseWheel(deltaX, deltaY));
    }
    SendKeyDown(key: string) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new KeyDownDto(key)),
            () => MainRc.RCBrowserSockets.SendKeyDown(key));
    }
    SendKeyUp(key: string) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new KeyUpDto(key)),
            () => MainRc.RCBrowserSockets.SendKeyUp(key));
    }
    SendKeyPress(key: string) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new KeyPressDto(key)),
            () => MainRc.RCBrowserSockets.SendKeyPress(key));
    }

    SendCtrlAltDel() {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new CtrlAltDelDto()),
            () => MainRc.RCBrowserSockets.SendCtrlAltDel());
    }

    async SendFile(buffer: Uint8Array, fileName: string) {
        var messageId = CreateGUID();

        this.SendToAgent(() => MainRc.RtcSession.SendDto(new FileDto(null, fileName, messageId, false, true)),
            () => MainRc.RCBrowserSockets.SendFile(null, fileName, messageId, false, true));

        for (var i = 0; i < buffer.byteLength; i += 50_000) {

            await this.SendToAgentAsync(async () => {
                MainRc.RtcSession.SendDto(new FileDto(buffer.slice(i, i + 50_000), fileName, messageId, false, false));
                await When(() => MainRc.RtcSession.DataChannel.bufferedAmount == 0, 10);
            }, async () => {
                    await MainRc.RCBrowserSockets.SendFile(buffer.slice(i, i + 50_000), fileName, messageId, false, false);
            });

            if (i > 0) {
                FileTransferProgress.value = i / buffer.byteLength;
            }
        }

        this.SendToAgent(() => MainRc.RtcSession.SendDto(new FileDto(null, fileName, messageId, true, false)),
            () => MainRc.RCBrowserSockets.SendFile(null, fileName, messageId, true, false));
    }

    SendQualityChange(qualityLevel: number) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new QualityChangeDto(qualityLevel)),
            () => MainRc.RCBrowserSockets.SendQualityChange(qualityLevel));
    }
    SendAutoQualityAdjust(isOn: boolean) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new AutoQualityAdjustDto(isOn)),
            () => MainRc.RCBrowserSockets.SendAutoQualityAdjust(isOn));
    }
    SendToggleAudio(toggleOn: boolean) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new ToggleAudioDto(toggleOn)),
            () => MainRc.RCBrowserSockets.SendToggleAudio(toggleOn));
    };
    SendToggleBlockInput(toggleOn: boolean) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new ToggleBlockInputDto(toggleOn)),
            () => MainRc.RCBrowserSockets.SendToggleBlockInput(toggleOn));
    }
    SendClipboardTransfer(text: string, typeText: boolean) {
        this.SendToAgent(() => MainRc.RtcSession.SendDto(new ClipboardTransferDto(text, typeText)),
            () => MainRc.RCBrowserSockets.SendClipboardTransfer(text, typeText));
    }

    private IsWebRtcAvailable() {
        return MainRc.RtcSession.DataChannel && MainRc.RtcSession.DataChannel.readyState == "open";
    }

    private SendToAgent(rtcSend: () => void, websocketSend: () => void) {
        if (MainRc.RtcSession.DataChannel && MainRc.RtcSession.DataChannel.readyState == "open") {
            rtcSend();
        }
        else if (MainRc.RCBrowserSockets.Connection.connectionStarted) {
            websocketSend();
        }
    }

    private async SendToAgentAsync(rtcSend: () => Promise<any>, websocketSend: () => Promise<any>) {
        if (this.IsWebRtcAvailable()) {
            await rtcSend();
        }
        else {
            await websocketSend();
        }
    }

  
}