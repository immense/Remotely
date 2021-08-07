import * as UI from "./UI.js";
import { BaseDtoType } from "./Enums/BaseDtoType.js";
import { ViewerApp } from "./App.js";
import { Sound } from "./Sound.js";
import { ReceiveFile } from "./FileTransferService.js";
import { HandleCaptureReceived } from "./CaptureProcessor.js";
export class DtoMessageHandler {
    constructor() {
        this.MessagePack = window['msgpack5']();
    }
    ParseBinaryMessage(data) {
        var model = this.MessagePack.decode(data);
        switch (model.DtoType) {
            case BaseDtoType.AudioSample:
                this.HandleAudioSample(model);
                break;
            case BaseDtoType.CaptureFrame:
                this.HandleCaptureFrame(model);
                break;
            case BaseDtoType.ClipboardText:
                this.HandleClipboardText(model);
                break;
            case BaseDtoType.CursorChange:
                this.HandleCursorChange(model);
                break;
            case BaseDtoType.ScreenData:
                this.HandleScreenData(model);
                break;
            case BaseDtoType.ScreenSize:
                this.HandleScreenSize(model);
                break;
            case BaseDtoType.WindowsSessions:
                this.HandleWindowsSessions(model);
                break;
            case BaseDtoType.File:
                this.HandleFile(model);
            default:
                break;
        }
    }
    HandleAudioSample(audioSample) {
        Sound.Play(audioSample.Buffer);
    }
    HandleCaptureFrame(captureFrame) {
        HandleCaptureReceived(captureFrame);
    }
    HandleClipboardText(clipboardText) {
        ViewerApp.ClipboardWatcher.SetClipboardText(clipboardText.ClipboardText);
    }
    HandleCursorChange(cursorChange) {
        UI.UpdateCursor(cursorChange.ImageBytes, cursorChange.HotSpotX, cursorChange.HotSpotY, cursorChange.CssOverride);
    }
    HandleFile(file) {
        ReceiveFile(file);
    }
    HandleScreenData(screenDataDto) {
        document.title = `${screenDataDto.MachineName} - Remotely Session`;
        UI.ToggleConnectUI(false);
        UI.SetScreenSize(screenDataDto.ScreenWidth, screenDataDto.ScreenHeight);
        UI.UpdateDisplays(screenDataDto.SelectedDisplay, screenDataDto.DisplayNames);
    }
    HandleScreenSize(screenSizeDto) {
        UI.SetScreenSize(screenSizeDto.Width, screenSizeDto.Height);
    }
    HandleWindowsSessions(windowsSessionsDto) {
        UI.UpdateWindowsSessions(windowsSessionsDto.WindowsSessions);
    }
}
//# sourceMappingURL=DtoMessageHandler.js.map