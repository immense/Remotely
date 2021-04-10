import { BaseDto } from "./BaseDto.js";
import { BaseDtoType } from "../Enums/BaseDtoType.js";
import { WindowsSession } from "../Models/WindowsSession.js";


export interface AudioSampleDto extends BaseDto {
    Buffer: Uint8Array;
}

export interface CaptureFrameDto extends BaseDto {
    EndOfFrame: boolean;
    Left: number;
    Top: number;
    Width: number;
    Height: number;
    ImageBytes: Uint8Array;
}

export interface ClipboardTextDto extends BaseDto {
    ClipboardText: string;
}


export class ClipboardTransferDto implements BaseDto {
    constructor(text: string, typeText:boolean) {
        this.Text = text;
        this.TypeText = typeText;
    }

    Text: string;
    TypeText: boolean;
    DtoType: BaseDtoType = BaseDtoType.ClipboardTransfer;
}


export class CtrlAltDelDto implements BaseDto {
    DtoType: BaseDtoType = BaseDtoType.CtrlAltDel;
}

export interface CursorChangeDto extends BaseDto {
    ImageBytes: Uint8Array;
    HotSpotX: number;
    HotSpotY: number;
    CssOverride: string;
}

export class FileDto implements BaseDto {
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

    DtoType: BaseDtoType = BaseDtoType.File;
}

export class GenericDto implements BaseDto {
    constructor(type: BaseDtoType) {
        this.DtoType = type;
    }
    DtoType: BaseDtoType;
}

export class KeyDownDto implements BaseDto {
    constructor(key: string) {
        this.Key = key;
    }

    Key: string;
    DtoType: BaseDtoType = BaseDtoType.KeyDown;
}

export class KeyPressDto implements BaseDto {
    constructor(key: string) {
        this.Key = key;
    }

    Key: string;
    DtoType: BaseDtoType = BaseDtoType.KeyPress;
}

export class KeyUpDto implements BaseDto {
    constructor(key: string) {
        this.Key = key;
    }

    Key: string;
    DtoType: BaseDtoType = BaseDtoType.KeyUp;
}

export interface MachineNameDto extends BaseDto {
    MachineName: string;
}

export class MouseDownDto implements BaseDto {
    constructor(button: number, percentX: number, percentY: number) {
        this.Button = button;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }

    Button: number;
    PercentX: number;
    PercentY: number;
    DtoType: BaseDtoType = BaseDtoType.MouseDown;
}

export class MouseMoveDto implements BaseDto {
    constructor(percentX: number, percentY: number) {
        this.PercentX = percentX;
        this.PercentY = percentY;
    }

    PercentX: number;
    PercentY: number;
    DtoType: BaseDtoType = BaseDtoType.MouseMove;
}

export class MouseUpDto implements BaseDto {
    constructor(button: number, percentX: number, percentY: number) {
        this.Button = button;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }

    Button: number;
    PercentX: number;
    PercentY: number;
    DtoType: BaseDtoType = BaseDtoType.MouseUp;
}

export class MouseWheelDto implements BaseDto {
    constructor(deltaX: number, deltaY: number) {
        this.DeltaX = deltaX;
        this.DeltaY = deltaY;
    }

    DeltaX: number;
    DeltaY: number;
    DtoType: BaseDtoType = BaseDtoType.MouseWheel;
}

export interface ScreenDataDto extends BaseDto {
    DisplayNames: string[];
    SelectedScreen: string;
}

export interface ScreenSizeDto extends BaseDto {
    Width: number;
    Height: number;
}

export class SelectScreenDto implements BaseDto {
    constructor(displayName: string) {
        this.DisplayName = displayName;
    }

    DisplayName: string;
    DtoType: BaseDtoType = BaseDtoType.SelectScreen;
}

export class TapDto implements BaseDto {
    constructor(percentX: number, percentY: number) {
        this.PercentX = percentX;
        this.PercentY = percentY;
    }

    PercentX: number;
    PercentY: number;
    DtoType: BaseDtoType = BaseDtoType.Tap;
}

export class ToggleAudioDto implements BaseDto {
    constructor(toggleOn: boolean) {
        this.ToggleOn = toggleOn;
    }

    ToggleOn: boolean;
    DtoType: BaseDtoType = BaseDtoType.ToggleAudio;
}

export class ToggleBlockInputDto implements BaseDto {
    constructor(toggleOn: boolean) {
        this.ToggleOn = toggleOn;
    }

    ToggleOn: boolean;
    DtoType: BaseDtoType = BaseDtoType.ToggleBlockInput;
}

export class ToggleWebRtcVideoDto implements BaseDto {
    constructor(toggleOn: boolean) {
        this.ToggleOn = toggleOn;
    }

    ToggleOn: boolean;
    DtoType: BaseDtoType = BaseDtoType.ToggleWebRtcVideo;
}


export class WindowsSessionsDto implements BaseDto {
    
    WindowsSessions: Array<WindowsSession>;
    DtoType: BaseDtoType = BaseDtoType.WindowsSessions;
}