import * as UI from "./UI.js";
import { DtoType } from "./Enums/DtoType.js";
import { ViewerApp } from "./App.js";
import { ShowToast } from "./UI.js";
import { Sound } from "./Sound.js";
import {
    AudioSampleDto,
    ClipboardTextDto,
    CursorChangeDto,
    ScreenDataDto,
    ScreenSizeDto,
    FileDto,
    WindowsSessionsDto,
    DtoWrapper,
    SessionMetricsDto
} from "./Interfaces/Dtos.js";
import { ReceiveFile } from "./FileTransferService.js";
import { TryComplete } from "./DtoChunker.js";
import { MessagePack } from "./Interfaces/MessagePack.js";

const MsgPack: MessagePack = window["MessagePack"];

export class DtoMessageHandler {

    async ParseBinaryMessage(data: ArrayBuffer) {
        var wrapper = MsgPack.decode<DtoWrapper>(data);
        switch (wrapper.DtoType) {
            case DtoType.AudioSample:
                await this.HandleAudioSample(wrapper);
                break;
            case DtoType.ClipboardText:
                this.HandleClipboardText(wrapper);
                break;
            case DtoType.CursorChange:
                this.HandleCursorChange(wrapper);
                break;
            case DtoType.ScreenData:
                this.HandleScreenData(wrapper);
                break;
            case DtoType.ScreenSize:
                this.HandleScreenSize(wrapper)
                break;
            case DtoType.WindowsSessions:
                this.HandleWindowsSessions(wrapper)
                break;
            case DtoType.File:
                this.HandleFile(wrapper);
            case DtoType.SessionMetrics:
                await this.HandleSessionMetrics(wrapper);
            default:
                break;
        }
    }

    async HandleAudioSample(wrapper: DtoWrapper) {
        let audioSample = TryComplete<AudioSampleDto>(wrapper);

        if (!audioSample) {
            return;
        }

        await Sound.Play(audioSample.Buffer);
    }

    HandleClipboardText(wrapper: DtoWrapper) {
        let clipboardText = TryComplete<ClipboardTextDto>(wrapper);

        if (!clipboardText) {
            return;
        }

        ViewerApp.ClipboardWatcher.SetClipboardText(clipboardText.ClipboardText);
    }
    HandleCursorChange(wrapper: DtoWrapper) {
        let cursorChange = TryComplete<CursorChangeDto>(wrapper);

        if (!cursorChange) {
            return;
        }

        UI.UpdateCursor(cursorChange.ImageBytes, cursorChange.HotSpotX, cursorChange.HotSpotY, cursorChange.CssOverride);
    }
    HandleFile(wrapper: DtoWrapper) {
        let file = TryComplete<FileDto>(wrapper);

        if (!file) {
            return;
        }

        ReceiveFile(file);
    }
    HandleScreenData(wrapper: DtoWrapper) {
        let screenDataDto = TryComplete<ScreenDataDto>(wrapper);

        if (!screenDataDto) {
            return;
        }

        document.title = `${screenDataDto.MachineName} - Remote Control Session`;
        UI.ToggleConnectUI(false);
        UI.SetScreenSize(screenDataDto.ScreenWidth, screenDataDto.ScreenHeight);
        UI.UpdateDisplays(screenDataDto.SelectedDisplay, screenDataDto.DisplayNames);
    }

    HandleScreenSize(wrapper: DtoWrapper) {
        let screenSizeDto = TryComplete<ScreenSizeDto>(wrapper);

        if (!screenSizeDto) {
            return;
        }

        UI.SetScreenSize(screenSizeDto.Width, screenSizeDto.Height);
    }

    async HandleSessionMetrics(wrapper: DtoWrapper) {
        let metricsDto = TryComplete<SessionMetricsDto>(wrapper);

        if (!metricsDto) {
            return;
        }

        UI.UpdateMetrics(metricsDto);
    }

    HandleWindowsSessions(wrapper: DtoWrapper) {
        let windowsSessionsDto = TryComplete<WindowsSessionsDto>(wrapper);

        if (!windowsSessionsDto) {
            return;
        }

        UI.UpdateWindowsSessions(windowsSessionsDto.WindowsSessions);
    }
}
