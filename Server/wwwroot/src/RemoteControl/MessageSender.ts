import { ViewerApp } from "./App.js";
import {
    CtrlAltDelDto,
    KeyDownDto,
    KeyPressDto,
    KeyUpDto,
    MouseDownDto,
    MouseMoveDto,
    MouseUpDto,
    MouseWheelDto,
    SelectScreenDto,
    TapDto,
    ToggleAudioDto,
    ToggleBlockInputDto,
    ClipboardTransferDto,
    FileDto,
    WindowsSessionsDto,
    GenericDto,
    ToggleWebRtcVideoDto
} from "./Interfaces/Dtos.js";
import { CreateGUID, When } from "./Utilities.js";
import { FileTransferProgress } from "./UI.js";
import { BaseDtoType } from "./Enums/BaseDtoType.js";
import { RemoteControlMode } from "./Enums/RemoteControlMode.js";

export class MessageSender {
    GetWindowsSessions() {
        if (ViewerApp.Mode == RemoteControlMode.Unattended) {
            var dto = new WindowsSessionsDto();
            this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
                () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
        }
    }
    ChangeWindowsSession(sessionId: number) {
        ViewerApp.ViewerHubConnection.ChangeWindowsSession(sessionId);
    }
    SendFrameReceived() {
        var dto = new GenericDto(BaseDtoType.FrameReceived);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendSelectScreen(displayName: string) {
        var dto = new SelectScreenDto(displayName);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendMouseMove(percentX: number, percentY: number) {
        var dto = new MouseMoveDto(percentX, percentY);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendMouseDown(button: number, percentX: number, percentY: number) {
        var dto = new MouseDownDto(button, percentX, percentY);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendMouseUp(button: number, percentX: number, percentY: number) {
        var dto = new MouseUpDto(button, percentX, percentY);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendTap(percentX: number, percentY: number) {
        var dto = new TapDto(percentX, percentY);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendMouseWheel(deltaX: number, deltaY: number) {
        var dto = new MouseWheelDto(deltaX, deltaY);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendKeyDown(key: string) {
        var dto = new KeyDownDto(key);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendKeyUp(key: string) {
        var dto = new KeyUpDto(key);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendKeyPress(key: string) {
        var dto = new KeyPressDto(key);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendSetKeyStatesUp() {
        var dto = new GenericDto(BaseDtoType.SetKeyStatesUp);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendCtrlAltDel() {
        var dto = new CtrlAltDelDto();
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }

    SendOpenFileTransferWindow() {
        var dto = new GenericDto(BaseDtoType.OpenFileTransferWindow);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }

    async SendFile(buffer: Uint8Array, fileName: string) {
        var messageId = CreateGUID();
        let dto = new FileDto(null, fileName, messageId, false, true);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));

        for (var i = 0; i < buffer.byteLength; i += 50_000) {

            let dto = new FileDto(buffer.slice(i, i + 50_000), fileName, messageId, false, false);

            await this.SendToAgentAsync(async () => {
                ViewerApp.RtcSession.SendDto(dto);
                await When(() => ViewerApp.RtcSession.DataChannel.bufferedAmount == 0, 10);
            }, async () => {
                await ViewerApp.ViewerHubConnection.SendDtoToClient(dto);
            });

            if (i > 0) {
                FileTransferProgress.value = i / buffer.byteLength;
            }
        }

        dto = new FileDto(null, fileName, messageId, true, false);

        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }

    SendToggleAudio(toggleOn: boolean) {
        var dto = new ToggleAudioDto(toggleOn);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    };
    SendToggleBlockInput(toggleOn: boolean) {
        var dto = new ToggleBlockInputDto(toggleOn);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendToggleWebRtcVideo(toggleOn: boolean) {
        var dto = new ToggleWebRtcVideoDto(toggleOn);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendClipboardTransfer(text: string, typeText: boolean) {
        var dto = new ClipboardTransferDto(text, typeText);
        this.SendToAgent(() => ViewerApp.RtcSession.SendDto(dto),
            () => ViewerApp.ViewerHubConnection.SendDtoToClient(dto));
    }

    private IsWebRtcAvailable() {
        return ViewerApp.RtcSession.DataChannel && ViewerApp.RtcSession.DataChannel.readyState == "open";
    }

    private SendToAgent(rtcSend: () => void, websocketSend: () => void) {
        if (ViewerApp.RtcSession.DataChannel && ViewerApp.RtcSession.DataChannel.readyState == "open") {
            rtcSend();
        }
        else if (ViewerApp.ViewerHubConnection.Connection?.connectionStarted) {
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