export class DtoWrapper {
}
export class EmptyDto {
}
export class TextTransferDto {
    constructor(text, typeText) {
        this.Text = text;
        this.TypeText = typeText;
    }
}
export class CtrlAltDelDto {
}
export class FileDto {
    constructor(buffer, fileName, messageId, endOfFile, startOfFile) {
        this.Buffer = buffer;
        this.FileName = fileName;
        this.MessageId = messageId;
        this.EndOfFile = endOfFile;
        this.StartOfFile = startOfFile;
    }
}
export class KeyDownDto {
    constructor(key) {
        this.Key = key;
    }
}
export class KeyPressDto {
    constructor(key) {
        this.Key = key;
    }
}
export class KeyUpDto {
    constructor(key) {
        this.Key = key;
    }
}
export class MouseDownDto {
    constructor(button, percentX, percentY) {
        this.Button = button;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }
}
export class MouseMoveDto {
    constructor(percentX, percentY) {
        this.PercentX = percentX;
        this.PercentY = percentY;
    }
}
export class MouseUpDto {
    constructor(button, percentX, percentY) {
        this.Button = button;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }
}
export class MouseWheelDto {
    constructor(deltaX, deltaY) {
        this.DeltaX = deltaX;
        this.DeltaY = deltaY;
    }
}
export class SelectScreenDto {
    constructor(displayName) {
        this.DisplayName = displayName;
    }
}
export class FrameReceivedDto {
    constructor(timestamp) {
        this.Timestamp = timestamp;
    }
}
export class TapDto {
    constructor(percentX, percentY) {
        this.PercentX = percentX;
        this.PercentY = percentY;
    }
}
export class ToggleAudioDto {
    constructor(toggleOn) {
        this.ToggleOn = toggleOn;
    }
}
export class ToggleBlockInputDto {
    constructor(toggleOn) {
        this.ToggleOn = toggleOn;
    }
}
export class WindowsSessionsDto {
}
//# sourceMappingURL=Dtos.js.map