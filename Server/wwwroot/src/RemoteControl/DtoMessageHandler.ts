import * as UI from "./UI.js";
import { BaseDtoType } from "./Enums/BaseDtoType.js";
import { BaseDto } from "./Interfaces/BaseDto.js";
import { ViewerApp } from "./App.js";
import { ShowMessage } from "./UI.js";
import { Sound } from "./Sound.js";
import {
    AudioSampleDto,
    CaptureFrameDto,
    ClipboardTextDto,
    CursorChangeDto,
    ScreenDataDto,
    ScreenSizeDto,
    FileDto,
    WindowsSessionsDto
} from "./Interfaces/Dtos.js";
import { ReceiveFile } from "./FileTransferService.js";
import { HandleCaptureReceived } from "./CaptureProcessor.js";

export class DtoMessageHandler {
    MessagePack: any = window['msgpack5']();

    ParseBinaryMessage(data: ArrayBuffer) {
        var model = this.MessagePack.decode(data) as BaseDto;
        switch (model.DtoType) {
            case BaseDtoType.AudioSample:
                this.HandleAudioSample(model as unknown as AudioSampleDto);
                break;
            case BaseDtoType.CaptureFrame:
                this.HandleCaptureFrame(model as unknown as CaptureFrameDto);
                break;
            case BaseDtoType.ClipboardText:
                this.HandleClipboardText(model as unknown as ClipboardTextDto);
                break;
            case BaseDtoType.CursorChange:
                this.HandleCursorChange(model as unknown as CursorChangeDto);
                break;
            case BaseDtoType.ScreenData:
                this.HandleScreenData(model as unknown as ScreenDataDto);
                break;
            case BaseDtoType.ScreenSize:
                this.HandleScreenSize(model as unknown as ScreenSizeDto)
                break;
            case BaseDtoType.WindowsSessions:
                this.HandleWindowsSessions(model as unknown as WindowsSessionsDto)
                break;
            case BaseDtoType.File:
                this.HandleFile(model as unknown as FileDto);
            default:
                break;
        }
    }

    HandleAudioSample(audioSample: AudioSampleDto) {
        Sound.Play(audioSample.Buffer);
    }

    HandleCaptureFrame(captureFrame: CaptureFrameDto) {
        HandleCaptureReceived(captureFrame);
    }

    HandleClipboardText(clipboardText: ClipboardTextDto) {
        ViewerApp.ClipboardWatcher.SetClipboardText(clipboardText.ClipboardText);
    }
    HandleCursorChange(cursorChange: CursorChangeDto) {
        UI.UpdateCursor(cursorChange.ImageBytes, cursorChange.HotSpotX, cursorChange.HotSpotY, cursorChange.CssOverride);
    }
    HandleFile(file: FileDto) {
        ReceiveFile(file);
    }
    HandleScreenData(screenDataDto: ScreenDataDto) {
        document.title = `${screenDataDto.MachineName} - Remotely Session`;
        UI.ToggleConnectUI(false);
        UI.SetScreenSize(screenDataDto.ScreenWidth, screenDataDto.ScreenHeight);
        UI.UpdateDisplays(screenDataDto.SelectedDisplay, screenDataDto.DisplayNames);
    }

    HandleScreenSize(screenSizeDto: ScreenSizeDto) {
        UI.SetScreenSize(screenSizeDto.Width, screenSizeDto.Height);
    }

    HandleWindowsSessions(windowsSessionsDto: WindowsSessionsDto) {
        UI.UpdateWindowsSessions(windowsSessionsDto.WindowsSessions);
    }
}