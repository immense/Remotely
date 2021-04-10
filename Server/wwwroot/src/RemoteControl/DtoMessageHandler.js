import * as UI from "./UI.js";
import { BaseDtoType } from "./Enums/BaseDtoType.js";
import { ViewerApp } from "./App.js";
import { ShowMessage } from "./UI.js";
import { Sound } from "./Sound.js";
import { ReceiveFile } from "./FileTransferService.js";
export class DtoMessageHandler {
    constructor() {
        this.MessagePack = window['MessagePack'];
        this.ImagePartials = [];
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
            case BaseDtoType.MachineName:
                this.HandleMachineName(model);
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
        if (captureFrame.EndOfFrame) {
            let completedFrame = new Blob(this.ImagePartials);
            this.ImagePartials = [];
            let url = window.URL.createObjectURL(completedFrame);
            let img = new Image(captureFrame.Width, captureFrame.Height);
            img.onload = () => {
                UI.Screen2DContext.drawImage(img, captureFrame.Left, captureFrame.Top, captureFrame.Width, captureFrame.Height);
                window.URL.revokeObjectURL(url);
            };
            img.src = url;
            //createImageBitmap(completedFrame).then(bitmap => {
            //    UI.Screen2DContext.drawImage(bitmap,
            //        captureFrame.Left,
            //        captureFrame.Top,
            //        captureFrame.Width,
            //        captureFrame.Height);
            //    bitmap.close();
            //})
            ViewerApp.MessageSender.SendFrameReceived();
        }
        else {
            this.ImagePartials.push(captureFrame.ImageBytes);
        }
    }
    HandleClipboardText(clipboardText) {
        ViewerApp.ClipboardWatcher.SetClipboardText(clipboardText.ClipboardText);
        ShowMessage("Clipboard updated.");
    }
    HandleCursorChange(cursorChange) {
        UI.UpdateCursor(cursorChange.ImageBytes, cursorChange.HotSpotX, cursorChange.HotSpotY, cursorChange.CssOverride);
    }
    HandleFile(file) {
        ReceiveFile(file);
    }
    HandleMachineName(machineNameDto) {
        document.title = `${machineNameDto.MachineName} - Remotely Session`;
    }
    HandleScreenData(screenDataDto) {
        UI.UpdateDisplays(screenDataDto.SelectedScreen, screenDataDto.DisplayNames);
    }
    HandleScreenSize(screenSizeDto) {
        UI.SetScreenSize(screenSizeDto.Width, screenSizeDto.Height);
    }
    HandleWindowsSessions(windowsSessionsDto) {
        UI.UpdateWindowsSessions(windowsSessionsDto.WindowsSessions);
    }
}
//# sourceMappingURL=DtoMessageHandler.js.map