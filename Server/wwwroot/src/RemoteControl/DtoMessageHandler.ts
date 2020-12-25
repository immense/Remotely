import * as UI from "./UI.js";
import { BaseDtoType } from "../Shared/Enums/BaseDtoType.js";
import { BaseDto } from "./Interfaces/BaseDto.js";
import { ViewerApp } from "./App.js";
import { ShowMessage } from "../Shared/UI.js";
import { Sound } from "../Shared/Sound.js";
import {
    AudioSampleDto,
    CaptureFrameDto,
    ClipboardTextDto,
    CursorChangeDto,
    MachineNameDto,
    ScreenDataDto,
    ScreenSizeDto,
    FileDto,
    WindowsSessionsDto
} from "./Interfaces/Dtos.js";
import { ReceiveFile } from "./FileTransferService.js";

export class DtoMessageHandler {
    MessagePack: any = window['MessagePack'];
    PartialCaptures: Record<string, CaptureFrameDto[]> = {};
    async ParseBinaryMessage(data: ArrayBuffer) {
        var model = this.MessagePack.decode(data) as BaseDto;
        switch (model.DtoType) {
            case BaseDtoType.AudioSample:
                this.HandleAudioSample(model as unknown as AudioSampleDto);
                break;
            case BaseDtoType.CaptureFrame:
                await this.HandleCaptureFrame(model as unknown as CaptureFrameDto);
                break;
            case BaseDtoType.ClipboardText:
                this.HandleClipboardText(model as unknown as ClipboardTextDto);
                break;
            case BaseDtoType.CursorChange:
                this.HandleCursorChange(model as unknown as CursorChangeDto);
                break;
            case BaseDtoType.MachineName:
                this.HandleMachineName(model as unknown as MachineNameDto);
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
    
    async HandleCaptureFrame(captureFrame: CaptureFrameDto) {
        if (UI.AutoQualityAdjustCheckBox.checked &&
            Number(UI.QualitySlider.value) != captureFrame.ImageQuality) {
            UI.QualitySlider.value = String(captureFrame.ImageQuality);
        }

        if (captureFrame.EndOfCapture) {
            ViewerApp.MessageSender.SendFrameReceived();

            Object.keys(this.PartialCaptures).forEach(x => {
                let partial = this.PartialCaptures[x];
                let firstFrame = partial[0];
                let frameBytes = partial.map(x => x.ImageBytes);

                var url = window.URL.createObjectURL(new Blob(frameBytes));
                var img = document.createElement("img");
                img.onload = () => {
                    UI.Screen2DContext.drawImage(img,
                        firstFrame.Left,
                        firstFrame.Top,
                        firstFrame.Width,
                        firstFrame.Height);
                    window.URL.revokeObjectURL(url);
                };
                img.src = url;
            })

            this.PartialCaptures = {};
        }
        //else if (captureFrame.EndOfFrame) {
        //    let key = `${captureFrame.Left},${captureFrame.Top}`;
        //    let frameBytes = this.PartialCaptures[key].map(x => x.ImageBytes);

        //    //var url = window.URL.createObjectURL(new Blob(frameBytes));
        //    //var img = document.createElement("img");
        //    //img.onload = () => {
        //    //    UI.StagingRenderer.drawImage(img,
        //    //        captureFrame.Left,
        //    //        captureFrame.Top,
        //    //        captureFrame.Width,
        //    //        captureFrame.Height);
        //    //    window.URL.revokeObjectURL(url);
        //    //};
        //    //img.src = url;


        //    let bitmap = await createImageBitmap(new Blob(frameBytes));

        //    UI.StagingRenderer.drawImage(bitmap,
        //        captureFrame.Left,
        //        captureFrame.Top,
        //        captureFrame.Width,
        //        captureFrame.Height);

        //    bitmap.close();
        //}
        else {
            let key = `${captureFrame.Left},${captureFrame.Top}`;
            if (this.PartialCaptures[key]) {
                this.PartialCaptures[key].push(captureFrame);
            }
            else {
                this.PartialCaptures[key] = [captureFrame];
            }
        }
    }

    HandleClipboardText(clipboardText: ClipboardTextDto) {
        ViewerApp.ClipboardWatcher.SetClipboardText(clipboardText.ClipboardText);
        ShowMessage("Clipboard updated.");
    }
    HandleCursorChange(cursorChange: CursorChangeDto) {
        UI.UpdateCursor(cursorChange.ImageBytes, cursorChange.HotSpotX, cursorChange.HotSpotY, cursorChange.CssOverride);
    }
    HandleFile(file: FileDto) {
        ReceiveFile(file);
    }
    HandleMachineName(machineNameDto: MachineNameDto) {
        document.title = `${machineNameDto.MachineName} - Remotely Session`;
    }
    HandleScreenData(screenDataDto: ScreenDataDto) {
        UI.UpdateDisplays(screenDataDto.SelectedScreen, screenDataDto.DisplayNames);
        ViewerApp.MessageSender.SendAutoQualityAdjust(ViewerApp.Settings.autoQualityEnabled);
        ViewerApp.MessageSender.SendQualityChange(ViewerApp.Settings.qualityLevel);
    }

    HandleScreenSize(screenSizeDto: ScreenSizeDto) {
        UI.SetScreenSize(screenSizeDto.Width, screenSizeDto.Height);
    }

    HandleWindowsSessions(windowsSessionsDto: WindowsSessionsDto) {
        UI.UpdateWindowsSessions(windowsSessionsDto.WindowsSessions);
    }
}