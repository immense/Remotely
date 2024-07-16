import { DtoType } from "../Enums/DtoType.js";
import { WindowsSession } from "../Models/WindowsSession.js";

export class DtoWrapper {
    DtoChunk: Uint8Array;
    DtoType: DtoType;
    IsFirstChunk: boolean;
    IsLastChunk: boolean;
    RequestId: string;
    InstanceId: string;
    SequenceId: number;
}

export class EmptyDto {

}

export interface AudioSampleDto {
    Buffer: Uint8Array;
}

export interface ClipboardTextDto {
    ClipboardText: string;
}


export class TextTransferDto {
    constructor(text: string, typeText:boolean) {
        this.Text = text;
        this.TypeText = typeText;
    }

    Text: string;
    TypeText: boolean;
}


export class CtrlAltDelDto {
}

export interface CursorChangeDto {
    ImageBytes: Uint8Array;
    HotSpotX: number;
    HotSpotY: number;
    CssOverride: string;
}

export class FileDto {
    constructor(buffer: Uint8Array,
        fileName: string,
        messageId: string,
        endOfFile: boolean,
        startOfFile: boolean) {

        this.Buffer = buffer;
        this.FileName = fileName;
        this.MessageId = messageId;
        this.EndOfFile = endOfFile
        this.StartOfFile = startOfFile;
    }

    Buffer: Uint8Array;
    FileName: string;
    MessageId: string;
    EndOfFile: boolean;
    StartOfFile: boolean;
}

export class KeyDownDto {
    constructor(key: string) {
        this.Key = key;
    }

    Key: string;
}

export class KeyPressDto {
    constructor(key: string) {
        this.Key = key;
    }

    Key: string;
}

export class KeyUpDto {
    constructor(key: string) {
        this.Key = key;
    }

    Key: string;
}

export class MouseDownDto {
    constructor(button: number, percentX: number, percentY: number) {
        this.Button = button;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }

    Button: number;
    PercentX: number;
    PercentY: number;
}

export class MouseMoveDto {
    constructor(percentX: number, percentY: number) {
        this.PercentX = percentX;
        this.PercentY = percentY;
    }

    PercentX: number;
    PercentY: number;
}

export class MouseUpDto {
    constructor(button: number, percentX: number, percentY: number) {
        this.Button = button;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }

    Button: number;
    PercentX: number;
    PercentY: number;
}

export class MouseWheelDto {
    constructor(deltaX: number, deltaY: number) {
        this.DeltaX = deltaX;
        this.DeltaY = deltaY;
    }

    DeltaX: number;
    DeltaY: number;
}

export interface RemoteControlViewerOptions {
    ShouldRecordSession: boolean;
}

export interface ScreenDataDto {
    DisplayNames: string[];
    SelectedDisplay: string;
    MachineName: string;
    ScreenWidth: number;
    ScreenHeight: number;
}

export interface ScreenSizeDto {
    Width: number;
    Height: number;
}

export class SelectScreenDto {
    constructor(displayName: string) {
        this.DisplayName = displayName;
    }

    DisplayName: string;
}

export class FrameReceivedDto {
    constructor(timestamp: number) {
        this.Timestamp = timestamp;
    }

    Timestamp: number;
}

export interface SessionMetricsDto {
    Mbps: number;
    Fps: number;
    RoundTripLatency: number;
    IsGpuAccelerated: boolean;
}

export class TapDto {
    constructor(percentX: number, percentY: number) {
        this.PercentX = percentX;
        this.PercentY = percentY;
    }

    PercentX: number;
    PercentY: number;
}

export class ToggleAudioDto {
    constructor(toggleOn: boolean) {
        this.ToggleOn = toggleOn;
    }

    ToggleOn: boolean;
}


export class ToggleBlockInputDto {
    constructor(toggleOn: boolean) {
        this.ToggleOn = toggleOn;
    }

    ToggleOn: boolean;
}


export class WindowsSessionsDto {
    
    WindowsSessions: Array<WindowsSession>;
}
