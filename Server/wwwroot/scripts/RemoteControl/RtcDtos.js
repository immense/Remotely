import { BinaryDtoType } from "../Enums/BinaryDtoType.js";
export class AutoQualityAdjustDto {
    constructor(isOn) {
        this.DtoType = BinaryDtoType.AutoQualityAdjust;
        this.IsOn = isOn;
    }
}
export class ClipboardTransferDto {
    constructor(text, typeText) {
        this.DtoType = BinaryDtoType.ClipboardTransfer;
        this.Text = text;
        this.TypeText = typeText;
    }
}
export class CtrlAltDelDto {
    constructor() {
        this.DtoType = BinaryDtoType.CtrlAltDel;
    }
}
export class FileDto {
    constructor(buffer, fileName, messageId, endOfFile, startOfFile) {
        this.DtoType = BinaryDtoType.File;
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
    ;
}
export class KeyDownDto {
    constructor(key) {
        this.DtoType = BinaryDtoType.KeyDown;
        this.Key = key;
    }
}
export class KeyPressDto {
    constructor(key) {
        this.DtoType = BinaryDtoType.KeyPress;
        this.Key = key;
    }
}
export class KeyUpDto {
    constructor(key) {
        this.DtoType = BinaryDtoType.KeyUp;
        this.Key = key;
    }
}
export class MouseDownDto {
    constructor(button, percentX, percentY) {
        this.DtoType = BinaryDtoType.MouseDown;
        this.Button = button;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }
}
export class MouseMoveDto {
    constructor(percentX, percentY) {
        this.DtoType = BinaryDtoType.MouseMove;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }
}
export class MouseUpDto {
    constructor(button, percentX, percentY) {
        this.DtoType = BinaryDtoType.MouseUp;
        this.Button = button;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }
}
export class MouseWheelDto {
    constructor(deltaX, deltaY) {
        this.DtoType = BinaryDtoType.MouseWheel;
        this.DeltaX = deltaX;
        this.DeltaY = deltaY;
    }
}
export class QualityChangeDto {
    constructor(qualityLevel) {
        this.DtoType = BinaryDtoType.QualityChange;
        this.QualityLevel = qualityLevel;
    }
}
export class SelectScreenDto {
    constructor(displayName) {
        this.DtoType = BinaryDtoType.SelectScreen;
        this.DisplayName = displayName;
    }
}
export class TapDto {
    constructor(percentX, percentY) {
        this.DtoType = BinaryDtoType.Tap;
        this.PercentX = percentX;
        this.PercentY = percentY;
    }
}
export class ToggleAudioDto {
    constructor(toggleOn) {
        this.DtoType = BinaryDtoType.ToggleAudio;
        this.ToggleOn = toggleOn;
    }
}
export class ToggleBlockInputDto {
    constructor(toggleOn) {
        this.DtoType = BinaryDtoType.ToggleBlockInput;
        this.ToggleOn = toggleOn;
    }
}
export class WindowsSessionsDto {
    constructor() {
        this.DtoType = BinaryDtoType.WindowsSessions;
    }
}
export var SessionType;
(function (SessionType) {
    SessionType[SessionType["Console"] = 0] = "Console";
    SessionType[SessionType["RDP"] = 1] = "RDP";
})(SessionType || (SessionType = {}));
export class WindowsSession {
}
//# sourceMappingURL=RtcDtos.js.map