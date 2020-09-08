import { MainViewer } from "./Main.js";
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
    FileDto,
    WindowsSessionsDto,
    GenericDto,
    ToggleWebRtcVideoDto
} from "./RtcDtos.js";
import { CreateGUID, When } from "../Shared/Utilities.js";
import { FileTransferProgress } from "./UI.js";
import { BinaryDtoType } from "../Shared/Enums/BinaryDtoType.js";

export class MessageSender {
    GetWindowsSessions() {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new WindowsSessionsDto()),
            () => MainViewer.ViewerHubConnection.GetWindowsSessions());
    }
    ChangeWindowsSession(sessionId: number) {
        MainViewer.ViewerHubConnection.ChangeWindowsSession(sessionId);
    }
    SendFrameReceived() {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new GenericDto(BinaryDtoType.FrameReceived)),
            () => MainViewer.ViewerHubConnection.SendFrameReceived());
    }
    SendSelectScreen(displayName: string) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new SelectScreenDto(displayName)),
            () => MainViewer.ViewerHubConnection.SendSelectScreen(displayName));
    }
    SendMouseMove(percentX: number, percentY: number) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new MouseMoveDto(percentX, percentY)),
            () => MainViewer.ViewerHubConnection.SendMouseMove(percentX, percentY));
    }
    SendMouseDown(button: number, percentX: number, percentY: number) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new MouseDownDto(button, percentX, percentY)),
            () => MainViewer.ViewerHubConnection.SendMouseDown(button, percentX, percentY));
    }
    SendMouseUp(button: number, percentX: number, percentY: number) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new MouseUpDto(button, percentX, percentY)),
            () => MainViewer.ViewerHubConnection.SendMouseUp(button, percentX, percentY));
    }
    SendTap(percentX: number, percentY: number) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new TapDto(percentX, percentY)),
            () => MainViewer.ViewerHubConnection.SendTap(percentX, percentY));
    }
    SendMouseWheel(deltaX: number, deltaY: number) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new MouseWheelDto(deltaX, deltaY)),
            () => MainViewer.ViewerHubConnection.SendMouseWheel(deltaX, deltaY));
    }
    SendKeyDown(key: string) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new KeyDownDto(key)),
            () => MainViewer.ViewerHubConnection.SendKeyDown(key));
    }
    SendKeyUp(key: string) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new KeyUpDto(key)),
            () => MainViewer.ViewerHubConnection.SendKeyUp(key));
    }
    SendKeyPress(key: string) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new KeyPressDto(key)),
            () => MainViewer.ViewerHubConnection.SendKeyPress(key));
    }
    SendSetKeyStatesUp() {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new GenericDto(BinaryDtoType.SetKeyStatesUp)),
            () => MainViewer.ViewerHubConnection.SendSetKeyStatesUp());
    }
    SendCtrlAltDel() {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new CtrlAltDelDto()),
            () => MainViewer.ViewerHubConnection.SendCtrlAltDel());
    }

    async SendFile(buffer: Uint8Array, fileName: string) {
        var messageId = CreateGUID();

        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new FileDto(null, fileName, messageId, false, true)),
            () => MainViewer.ViewerHubConnection.SendFile(null, fileName, messageId, false, true));

        for (var i = 0; i < buffer.byteLength; i += 50_000) {

            await this.SendToAgentAsync(async () => {
                MainViewer.RtcSession.SendDto(new FileDto(buffer.slice(i, i + 50_000), fileName, messageId, false, false));
                await When(() => MainViewer.RtcSession.DataChannel.bufferedAmount == 0, 10);
            }, async () => {
                    await MainViewer.ViewerHubConnection.SendFile(buffer.slice(i, i + 50_000), fileName, messageId, false, false);
            });

            if (i > 0) {
                FileTransferProgress.value = i / buffer.byteLength;
            }
        }

        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new FileDto(null, fileName, messageId, true, false)),
            () => MainViewer.ViewerHubConnection.SendFile(null, fileName, messageId, true, false));
    }

    SendQualityChange(qualityLevel: number) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new QualityChangeDto(qualityLevel)),
            () => MainViewer.ViewerHubConnection.SendQualityChange(qualityLevel));
    }
    SendAutoQualityAdjust(isOn: boolean) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new AutoQualityAdjustDto(isOn)),
            () => MainViewer.ViewerHubConnection.SendAutoQualityAdjust(isOn));
    }
    SendToggleAudio(toggleOn: boolean) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new ToggleAudioDto(toggleOn)),
            () => MainViewer.ViewerHubConnection.SendToggleAudio(toggleOn));
    };
    SendToggleBlockInput(toggleOn: boolean) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new ToggleBlockInputDto(toggleOn)),
            () => MainViewer.ViewerHubConnection.SendToggleBlockInput(toggleOn));
    }
    SendToggleWebRtcVideo(toggleOn: boolean) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new ToggleWebRtcVideoDto(toggleOn)),
            () => MainViewer.ViewerHubConnection.SendToggleWebRtcVideo(toggleOn));
    }
    SendClipboardTransfer(text: string, typeText: boolean) {
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(new ClipboardTransferDto(text, typeText)),
            () => MainViewer.ViewerHubConnection.SendClipboardTransfer(text, typeText));
    }

    private IsWebRtcAvailable() {
        return MainViewer.RtcSession.DataChannel && MainViewer.RtcSession.DataChannel.readyState == "open";
    }

    private SendToAgent(rtcSend: () => void, websocketSend: () => void) {
        if (MainViewer.RtcSession.DataChannel && MainViewer.RtcSession.DataChannel.readyState == "open") {
            rtcSend();
        }
        else if (MainViewer.ViewerHubConnection.Connection.connectionStarted) {
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