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
} from "./Dtos.js";
import { CreateGUID, When } from "../Shared/Utilities.js";
import { FileTransferProgress } from "./UI.js";
import { BaseDtoType } from "../Shared/Enums/BaseDtoType.js";
import { RemoteControlMode } from "../Shared/Enums/RemoteControlMode.js";

export class MessageSender {
    GetWindowsSessions() {
        if (MainViewer.Mode == RemoteControlMode.Unattended) {
            var dto = new WindowsSessionsDto();
            this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
                () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
        }
    }
    ChangeWindowsSession(sessionId: number) {
        MainViewer.ViewerHubConnection.ChangeWindowsSession(sessionId);
    }
    SendFrameReceived() {
        var dto = new GenericDto(BaseDtoType.FrameReceived);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendSelectScreen(displayName: string) {
        var dto = new SelectScreenDto(displayName);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendMouseMove(percentX: number, percentY: number) {
        var dto = new MouseMoveDto(percentX, percentY);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendMouseDown(button: number, percentX: number, percentY: number) {
        var dto = new MouseDownDto(button, percentX, percentY);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendMouseUp(button: number, percentX: number, percentY: number) {
        var dto = new MouseUpDto(button, percentX, percentY);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendTap(percentX: number, percentY: number) {
        var dto = new TapDto(percentX, percentY);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendMouseWheel(deltaX: number, deltaY: number) {
        var dto = new MouseWheelDto(deltaX, deltaY);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendKeyDown(key: string) {
        var dto = new KeyDownDto(key);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendKeyUp(key: string) {
        var dto = new KeyUpDto(key);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendKeyPress(key: string) {
        var dto = new KeyPressDto(key);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendSetKeyStatesUp() {
        var dto = new GenericDto(BaseDtoType.SetKeyStatesUp);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendCtrlAltDel() {
        var dto = new CtrlAltDelDto();
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }

    async SendFile(buffer: Uint8Array, fileName: string) {
        var messageId = CreateGUID();
        let dto = new FileDto(null, fileName, messageId, false, true);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));

        for (var i = 0; i < buffer.byteLength; i += 50_000) {

            let dto = new FileDto(buffer.slice(i, i + 50_000), fileName, messageId, false, false);

            await this.SendToAgentAsync(async () => {
                MainViewer.RtcSession.SendDto(dto);
                await When(() => MainViewer.RtcSession.DataChannel.bufferedAmount == 0, 10);
            }, async () => {
                    await MainViewer.ViewerHubConnection.SendDtoToClient(dto);
            });

            if (i > 0) {
                FileTransferProgress.value = i / buffer.byteLength;
            }
        }

        dto = new FileDto(null, fileName, messageId, true, false);

        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }

    SendQualityChange(qualityLevel: number) {
        var dto = new QualityChangeDto(qualityLevel);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendAutoQualityAdjust(isOn: boolean) {
        var dto = new AutoQualityAdjustDto(isOn);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendToggleAudio(toggleOn: boolean) {
        var dto = new ToggleAudioDto(toggleOn);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    };
    SendToggleBlockInput(toggleOn: boolean) {
        var dto = new ToggleBlockInputDto(toggleOn);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendToggleWebRtcVideo(toggleOn: boolean) {
        var dto = new ToggleWebRtcVideoDto(toggleOn);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
    }
    SendClipboardTransfer(text: string, typeText: boolean) {
        var dto = new ClipboardTransferDto(text, typeText);
        this.SendToAgent(() => MainViewer.RtcSession.SendDto(dto),
            () => MainViewer.ViewerHubConnection.SendDtoToClient(dto));
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