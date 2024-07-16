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
    TextTransferDto,
    FileDto,
    WindowsSessionsDto,
    DtoWrapper,
    EmptyDto,
    FrameReceivedDto
} from "./Interfaces/Dtos.js";
import { CreateGUID } from "./Utilities.js";
import { FileTransferProgress } from "./UI.js";
import { DtoType } from "./Enums/DtoType.js";
import { RemoteControlMode } from "./Enums/RemoteControlMode.js";

export class MessageSender {
    async GetWindowsSessions() {
        if (ViewerApp.Mode == RemoteControlMode.Unattended) {
            var dto = new WindowsSessionsDto();
            await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.WindowsSessions);
        }
    }
    async ChangeWindowsSession(sessionId: number) {
        await ViewerApp.ViewerHubConnection.ChangeWindowsSession(sessionId);
    }
    async SendFrameReceived(timestamp: number) {
        var dto = new FrameReceivedDto(timestamp);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.FrameReceived);
    }
    async SendSelectScreen(displayName: string) {
        var dto = new SelectScreenDto(displayName);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.SelectScreen);
    }
    async SendMouseMove(percentX: number, percentY: number) {
        var dto = new MouseMoveDto(percentX, percentY);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.MouseMove);
    }
    async SendMouseDown(button: number, percentX: number, percentY: number) {
        var dto = new MouseDownDto(button, percentX, percentY);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.MouseDown);
    }
    async SendMouseUp(button: number, percentX: number, percentY: number) {
        var dto = new MouseUpDto(button, percentX, percentY);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.MouseUp);
    }
    async SendTap(percentX: number, percentY: number) {
        var dto = new TapDto(percentX, percentY);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.Tap);
    }
    async SendMouseWheel(deltaX: number, deltaY: number) {
        var dto = new MouseWheelDto(deltaX, deltaY);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.MouseWheel);
    }
    async SendKeyDown(key: string) {
        var dto = new KeyDownDto(key);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.KeyDown);
    }
    async SendKeyUp(key: string) {
        var dto = new KeyUpDto(key);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.KeyUp);
    }
    async SendKeyPress(key: string) {
        var dto = new KeyPressDto(key);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.KeyPress);
    }
    async SendSetKeyStatesUp() {
        var dto = new EmptyDto();
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.SetKeyStatesUp);
    }
    async SendCtrlAltDel() {
        var dto = new CtrlAltDelDto();
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.CtrlAltDel);
        await ViewerApp.ViewerHubConnection.InvokeCtrlAltDel();
    }

    async SendOpenFileTransferWindow() {
        var dto = new EmptyDto();
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.OpenFileTransferWindow);
    }

    async SendFile(buffer: Uint8Array, fileName: string) {
        var messageId = CreateGUID();
        let dto = new FileDto(null, fileName, messageId, false, true);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.File);


        for (var i = 0; i < buffer.byteLength; i += 50_000) {

            let dto = new FileDto(buffer.slice(i, i + 50_000), fileName, messageId, false, false);

            await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.File);

            if (i > 0) {
                FileTransferProgress.value = i / buffer.byteLength;
            }
        }

        dto = new FileDto(null, fileName, messageId, true, false);

        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.File);

    }

    async SendToggleAudio(toggleOn: boolean) {
        var dto = new ToggleAudioDto(toggleOn);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.ToggleAudio);

    };
    async SendToggleBlockInput(toggleOn: boolean) {
        var dto = new ToggleBlockInputDto(toggleOn);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.ToggleBlockInput);

    }

    async SendTextTransfer(text: string, typeText: boolean) {
        var dto = new TextTransferDto(text, typeText);
        await ViewerApp.ViewerHubConnection.SendDtoToClient(dto, DtoType.TextTransfer);

    }
}
