import * as UI from "./UI.js";
import { DynamicDtoType } from "../Enums/DynamicDtoType.js";
import { Remotely } from "./Main.js";
export class RtcMessageHandler {
    constructor() {
        this.FpsStack = [];
        this.MessagePack = window['MessagePack'];
        this.PartialCaptureFrames = [];
    }
    ParseBinaryMessage(data) {
        var model = this.MessagePack.decode(data);
        switch (model.DtoType) {
            case DynamicDtoType.CaptureFrame:
                this.ProcessCaptureFrame(model);
                break;
            case DynamicDtoType.MachineName:
                this.ProcessMachineName(model);
                break;
            case DynamicDtoType.ScreenData:
                this.ProcessScreenData(model);
                break;
            case DynamicDtoType.ScreenSize:
                this.ProcessScreenSize(model);
                break;
            default:
        }
    }
    ProcessMachineName(machineNameDto) {
        document.title = `${machineNameDto.MachineName} - Remotely Session`;
    }
    ProcessScreenData(screenDataDto) {
        UI.UpdateDisplays(screenDataDto.SelectedScreen, screenDataDto.DisplayNames);
    }
    ProcessScreenSize(screenSizeDto) {
        UI.SetScreenSize(screenSizeDto.Width, screenSizeDto.Height);
    }
    ProcessCaptureFrame(captureFrame) {
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
            if (Remotely.Debug) {
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
}
//# sourceMappingURL=RtcMessageHandler.js.map