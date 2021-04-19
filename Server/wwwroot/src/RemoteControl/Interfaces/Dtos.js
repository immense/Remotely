import { BaseDtoType } from "../Enums/BaseDtoType.js";
export class ClipboardTransferDto {
    constructor(text, typeText) {
        this.DtoType = BaseDtoType.ClipboardTransfer;
        this.Text = text;
        this.TypeText = typeText;
    }
}
export class CtrlAltDelDto {
    constructor() {
        this.DtoType = BaseDtoType.CtrlAltDel;
    }
}
export class FileDto {
    constructor(buffer, fileName, messageId, endOfFile, startOfFile) {
        this.DtoType = BaseDtoType.File;
        this.Buffer = buffer;
        this.FileName = fileName;
        this.MessageId = messageId;
        this.EndOfFile = endOfFile;
        this.StartOfFile = startOfFile;
    }
}
export class GenericDto {
    constructor(type) {
        this.DtoType = type;
    }
}
export class KeyDownDto {
    constructor(key) {
        this.DtoType = BaseDtoType.KeyDown;
        this.Key = key;
    }
}
export class KeyPressDto {
    constructor(key) {
        this.DtoType = BaseDtoType.KeyPress;
        this.Key = key;
    }
}
export class KeyUpDto {
    constructor(key) {
        this.DtoType = BaseDtoType.KeyUp;
        this.Key = key;
    }
}
export class MouseDownDto {
    constructor(button, percentX, percentY) {
        this.DtoType = BaseDtoType.MouseDown;
        this.Button = button;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }
}
export class MouseMoveDto {
    constructor(percentX, percentY) {
        this.DtoType = BaseDtoType.MouseMove;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }
}
export class MouseUpDto {
    constructor(button, percentX, percentY) {
        this.DtoType = BaseDtoType.MouseUp;
        this.Button = button;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }
}
export class MouseWheelDto {
    constructor(deltaX, deltaY) {
        this.DtoType = BaseDtoType.MouseWheel;
        this.DeltaX = deltaX;
        this.DeltaY = deltaY;
    }
}
export class SelectScreenDto {
    constructor(displayName) {
        this.DtoType = BaseDtoType.SelectScreen;
        this.DisplayName = displayName;
    }
}
export class TapDto {
    constructor(percentX, percentY) {
        this.DtoType = BaseDtoType.Tap;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }
}
export class ToggleAudioDto {
    constructor(toggleOn) {
        this.DtoType = BaseDtoType.ToggleAudio;
        this.ToggleOn = toggleOn;
    }
}
export class ToggleBlockInputDto {
    constructor(toggleOn) {
        this.DtoType = BaseDtoType.ToggleBlockInput;
        this.ToggleOn = toggleOn;
    }
}
export class ToggleWebRtcVideoDto {
    constructor(toggleOn) {
        this.DtoType = BaseDtoType.ToggleWebRtcVideo;
        this.ToggleOn = toggleOn;
    }
}
export class WindowsSessionsDto {
    constructor() {
        this.DtoType = BaseDtoType.WindowsSessions;
    }
}
//# sourceMappingURL=Dtos.js.map