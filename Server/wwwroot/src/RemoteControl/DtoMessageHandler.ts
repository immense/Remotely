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
    MachineNameDto,
    ScreenDataDto,
    ScreenSizeDto,
    FileDto,
    WindowsSessionsDto
} from "./Interfaces/Dtos.js";
import { ReceiveFile } from "./FileTransferService.js";

export class DtoMessageHandler {
    MessagePack: any = window['MessagePack'];
    ImagePartials: Array<Uint8Array> = [];

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
    
    HandleCaptureFrame(captureFrame: CaptureFrameDto) {

        if (captureFrame.EndOfFrame) {
            let completedFrame = new Blob(this.ImagePartials);

            this.ImagePartials = [];

            let url = window.URL.createObjectURL(completedFrame);
            let img = new Image(captureFrame.Width, captureFrame.Height);
            img.onload = () => {
                UI.Screen2DContext.drawImage(img,
                    captureFrame.Left,
                    captureFrame.Top,
                    captureFrame.Width,
                    captureFrame.Height);
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
    }

    HandleScreenSize(screenSizeDto: ScreenSizeDto) {
        UI.SetScreenSize(screenSizeDto.Width, screenSizeDto.Height);
    }

    HandleWindowsSessions(windowsSessionsDto: WindowsSessionsDto) {
        UI.UpdateWindowsSessions(windowsSessionsDto.WindowsSessions);
    }
}