import * as UI from "./UI.js";
import { DynamicDtoType } from "../Enums/DynamicDtoType.js";
import { DynamicDto } from "./DynamicDto.js";
import { Remotely } from "./Main.js";
import { CaptureFrameDto } from "./RtcDtos/CaptureFrameDto.js";
import { MachineNameDto } from "./RtcDtos/MachineNameDto.js";
import { ScreenDataDto } from "./RtcDtos/ScreenDataDto.js";
import { ScreenSizeDto } from "./RtcDtos/ScreenSizeDto.js";

export class RtcMessageHandler {
    FpsStack: Array<number> = [];
    MessagePack: any = window['MessagePack'];
    PartialCaptureFrames: Uint8Array[] = [];
    ParseBinaryMessage(data: ArrayBuffer) {
        var model = this.MessagePack.decode(data) as DynamicDto;
        switch (model.DtoType) {
            case DynamicDtoType.CaptureFrame:
                this.ProcessCaptureFrame(model as unknown as CaptureFrameDto);
                break;
            case DynamicDtoType.MachineName:
                this.ProcessMachineName(model as unknown as MachineNameDto);
                break;
            case DynamicDtoType.ScreenData:
                this.ProcessScreenData(model as unknown as ScreenDataDto);
                break;
            case DynamicDtoType.ScreenSize:
                this.ProcessScreenSize(model as unknown as ScreenSizeDto)
                break;
            default:
        }
    }
    ProcessMachineName(machineNameDto: MachineNameDto) {
        document.title = `${machineNameDto.MachineName} - Remotely Session`;
    }
    ProcessScreenData(screenDataDto: ScreenDataDto) {
        UI.UpdateDisplays(screenDataDto.SelectedScreen, screenDataDto.DisplayNames);
    }

    ProcessScreenSize(screenSizeDto: ScreenSizeDto) {
        UI.SetScreenSize(screenSizeDto.Width, screenSizeDto.Height);
    }

    ProcessCaptureFrame(captureFrame: CaptureFrameDto) {
        if (UI.AutoQualityAdjustCheckBox.checked &&
            Number(UI.QualitySlider.value) != captureFrame.ImageQuality) {
            UI.QualitySlider.value = String(captureFrame.ImageQuality);
        }

        if (captureFrame.EndOfFrame) {
            var url = window.URL.createObjectURL(new Blob(this.PartialCaptureFrames));
            var img = document.createElement("img");
            img.onload = () => {
                UI.Screen2DContext.drawImage(img,
                    captureFrame.Left,
                    captureFrame.Top,
                    captureFrame.Width,
                    captureFrame.Height);
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