import * as UI from "./UI.js";
import { BinaryDtoType } from "../Enums/BinaryDtoType.js";
import { MainRc } from "./Main.js";
import { ShowMessage } from "../UI.js";
import { Sound } from "../Sound.js";
export class RtcMessageHandler {
    constructor() {
        this.FpsStack = [];
        this.MessagePack = window['MessagePack'];
        this.PartialCaptureFrames = [];
    }
    ParseBinaryMessage(data) {
        var model = this.MessagePack.decode(data);
        switch (model.DtoType) {
            case BinaryDtoType.AudioSample:
                this.HandleAudioSample(model);
                break;
            case BinaryDtoType.CaptureFrame:
                this.HandleCaptureFrame(model);
                break;
            case BinaryDtoType.ClipboardText:
                this.HandleClipboardText(model);
                break;
            case BinaryDtoType.CursorChange:
                this.HandleCursorChange(model);
                break;
            case BinaryDtoType.MachineName:
                this.HandleMachineName(model);
                break;
            case BinaryDtoType.ScreenData:
                this.HandleScreenData(model);
                break;
            case BinaryDtoType.ScreenSize:
                this.HandleScreenSize(model);
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
            var url = window.URL.createObjectURL(new Blob(this.PartialCaptureFrames));
            var img = document.createElement("img");
            img.onload = () => {
                UI.Screen2DContext.drawImage(img, captureFrame.Left, captureFrame.Top, captureFrame.Width, captureFrame.Height);
                window.URL.revokeObjectURL(url);
            };
            img.src = url;
            this.PartialCaptureFrames = [];
            if (MainRc.Debug) {
                this.FpsStack.push(Date.now());
                while (Date.now() - this.FpsStack[0] > 1000) {
                    this.FpsStack.shift();
                }
                console.log("FPS: " + String(this.FpsStack.length));
            }
        }
        else {
            this.PartialCaptureFrames.push(captureFrame.ImageBytes);
        }
    }
    HandleClipboardText(clipboardText) {
        MainRc.ClipboardWatcher.SetClipboardText(clipboardText.ClipboardText);
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
}
//# sourceMappingURL=RtcMessageHandler.js.map