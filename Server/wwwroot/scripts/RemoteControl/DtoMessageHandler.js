import * as UI from "./UI.js";
import { BaseDtoType } from "../Shared/Enums/BaseDtoType.js";
import { MainViewer } from "./Main.js";
import { ShowMessage } from "../Shared/UI.js";
import { Sound } from "../Shared/Sound.js";
export class DtoMessageHandler {
    constructor() {
        this.MessagePack = window['MessagePack'];
        this.PartialCaptureFrames = [];
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
            default:
                break;
        }
    }
    HandleAudioSample(audioSample) {
        Sound.Play(audioSample.Buffer);
    }
    HandleCaptureFrame(captureFrame) {
        if (UI.AutoQualityAdjustCheckBox.checked &&
            Number(UI.QualitySlider.value) != captureFrame.ImageQuality) {
            UI.QualitySlider.value = String(captureFrame.ImageQuality);
        }
        if (captureFrame.EndOfFrame) {
            MainViewer.MessageSender.SendFrameReceived();
            var url = window.URL.createObjectURL(new Blob(this.PartialCaptureFrames));
            var img = document.createElement("img");
            img.onload = () => {
                UI.Screen2DContext.drawImage(img, captureFrame.Left, captureFrame.Top, captureFrame.Width, captureFrame.Height);
                window.URL.revokeObjectURL(url);
            };
            img.src = url;
            this.PartialCaptureFrames = [];
        }
        else {
            this.PartialCaptureFrames.push(captureFrame.ImageBytes);
        }
    }
    HandleClipboardText(clipboardText) {
        MainViewer.ClipboardWatcher.SetClipboardText(clipboardText.ClipboardText);
        ShowMessage("Clipboard updated.");
    }
    HandleCursorChange(cursorChange) {
        UI.UpdateCursor(cursorChange.ImageBytes, cursorChange.HotSpotX, cursorChange.HotSpotY, cursorChange.CssOverride);
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