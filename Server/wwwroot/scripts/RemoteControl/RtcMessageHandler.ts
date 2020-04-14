import * as UI from "./UI.js";
import { BinaryDtoType } from "../Enums/BinaryDtoType.js";
import { BinaryDto } from "./BinaryDto.js";
import { MainRc } from "./Main.js";
import { PopupMessage } from "../UI.js";
import { Sound } from "../Sound.js";
import {
    AudioSampleDto,
    CaptureFrameDto,
    ClipboardTextDto,
    CursorChangeDto,
    MachineNameDto,
    ScreenDataDto,
    ScreenSizeDto
} from "./RtcDtos.js";


export class RtcMessageHandler {
    FpsStack: Array<number> = [];
    MessagePack: any = window['MessagePack'];
    PartialCaptureFrames: Uint8Array[] = [];
    ParseBinaryMessage(data: ArrayBuffer) {
        var model = this.MessagePack.decode(data) as BinaryDto;
        switch (model.DtoType) {
            case BinaryDtoType.AudioSample:
                this.HandleAudioSample(model as unknown as AudioSampleDto);
                break;
            case BinaryDtoType.CaptureFrame:
                this.HandleCaptureFrame(model as unknown as CaptureFrameDto);
                break;
            case BinaryDtoType.ClipboardText:
                this.HandleClipboardText(model as unknown as ClipboardTextDto);
                break;
            case BinaryDtoType.CursorChange:
                this.HandleCursorChange(model as unknown as CursorChangeDto);
                break;
            case BinaryDtoType.MachineName:
                this.HandleMachineName(model as unknown as MachineNameDto);
                break;
            case BinaryDtoType.ScreenData:
                this.HandleScreenData(model as unknown as ScreenDataDto);
                break;
            case BinaryDtoType.ScreenSize:
                this.HandleScreenSize(model as unknown as ScreenSizeDto)
                break;
            default:
                break;
        }
    }
    HandleAudioSample(audioSample: AudioSampleDto) {
        Sound.Play(audioSample.Buffer);
    }
    HandleCaptureFrame(captureFrame: CaptureFrameDto) {
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
    HandleClipboardText(clipboardText: ClipboardTextDto) {
        MainRc.ClipboardWatcher.SetClipboardText(clipboardText.ClipboardText);
        PopupMessage("Clipboard updated.");
    }
    HandleCursorChange(cursorChange: CursorChangeDto) {
        UI.UpdateCursor(cursorChange.CursorInfo);
    }
    HandleMachineName(machineNameDto: MachineNameDto) {
        document.title = `${machineNameDto.MachineName} - Remotely Session`;
    }
    HandleScreenData(screenDataDto: ScreenDataDto) {
        UI.UpdateDisplays(screenDataDto.SelectedScreen, screenDataDto.DisplayNames);
    }

    HandleScreenSize(screenSizeDto: ScreenSizeDto) {
        UI.SetScreenSize(screenSizeDto.Width, screenSizeDto.Height);
    }
}