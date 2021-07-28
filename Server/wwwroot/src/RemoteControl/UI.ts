import { ViewerApp } from "./App.js";
import { ConvertUInt8ArrayToBase64 } from "./Utilities.js";
import { WindowsSession } from "./Models/WindowsSession.js";
import { WindowsSessionType } from "./Enums/WindowsSessionType.js";
import { RemoteControlMode } from "./Enums/RemoteControlMode.js";

export var AudioButton = document.getElementById("audioButton") as HTMLButtonElement;
export var MenuButton = document.getElementById("menuButton") as HTMLButtonElement;
export var MenuFrame = document.getElementById("menuFrame") as HTMLDivElement;
export var SessionIDInput = document.getElementById("sessionIDInput") as HTMLInputElement;
export var ConnectButton = document.getElementById("connectButton") as HTMLButtonElement;
export var RequesterNameInput = document.getElementById("nameInput") as HTMLInputElement;
export var StatusMessage = document.getElementById("statusMessage") as HTMLDivElement;
export var ScreenViewer = document.getElementById("screenViewer") as HTMLCanvasElement;
export var VideoScreenViewer = document.getElementById("videoScreenViewer") as HTMLVideoElement;
export var ScreenViewerWrapper = document.getElementById("screenViewerWrapper") as HTMLDivElement;
export var Screen2DContext = ScreenViewer ? ScreenViewer.getContext("2d") : null;
export var HorizontalBars = document.querySelectorAll(".horizontal-button-bar");
export var ConnectBox = document.getElementById("connectBox") as HTMLDivElement;
export var DisconnectedBox = document.getElementById("disconnectedBox") as HTMLDivElement;
export var ScreenSelectBar = document.getElementById("screenSelectBar") as HTMLDivElement;
export var ActionsBar = document.getElementById("actionsBar") as HTMLDivElement;
export var ViewBar = document.getElementById("viewBar") as HTMLDivElement;
export var ChangeScreenButton = document.getElementById("changeScreenButton") as HTMLButtonElement;
export var StreamVideoButton = document.getElementById("streamVideoButton") as HTMLButtonElement;
export var FitToScreenButton = document.getElementById("fitToScreenButton") as HTMLButtonElement;
export var BlockInputButton = document.getElementById("blockInputButton") as HTMLButtonElement;
export var DisconnectButton = document.getElementById("disconnectButton") as HTMLButtonElement;
export var FileTransferInput = document.getElementById("fileTransferInput") as HTMLInputElement;
export var FileTransferProgress = document.getElementById("fileTransferProgress") as HTMLProgressElement;
export var FileTransferNameSpan = document.getElementById("fileTransferNameSpan") as HTMLSpanElement;
export var KeyboardButton = document.getElementById("keyboardButton") as HTMLButtonElement;
export var InviteButton = document.getElementById("inviteButton") as HTMLButtonElement;
export var FileTransferButton = document.getElementById("fileTransferButton") as HTMLButtonElement;
export var FileTransferBar = document.getElementById("fileTransferBar") as HTMLDivElement;
export var FileUploadButtton = document.getElementById("fileUploadButton") as HTMLButtonElement;
export var FileDownloadButton = document.getElementById("fileDownloadButton") as HTMLButtonElement;
export var CtrlAltDelButton = document.getElementById("ctrlAltDelButton") as HTMLButtonElement;
export var TouchKeyboardTextArea = document.getElementById("touchKeyboardTextArea") as HTMLTextAreaElement;
export var ClipboardTransferBar = document.getElementById("clipboardTransferBar") as HTMLDivElement;
export var ClipboardTransferButton = document.getElementById("clipboardTransferButton") as HTMLButtonElement;
export var TypeClipboardButton = document.getElementById("typeClipboardButton") as HTMLButtonElement;
export var ConnectionP2PIcon = document.getElementById("connectionP2PIcon") as HTMLElement;
export var ConnectionRelayedIcon = document.getElementById("connectionRelayedIcon") as HTMLElement;
export var WindowsSessionSelect = document.getElementById("windowsSessionSelect") as HTMLSelectElement;
export var RecordSessionButton = document.getElementById("recordSessionButton") as HTMLButtonElement;
export var DownloadRecordingButton = document.getElementById("downloadRecordingButton") as HTMLButtonElement;
export var ViewOnlyButton = document.getElementById("viewOnlyButton") as HTMLButtonElement;
export var FullScreenButton = document.getElementById("fullScreenButton") as HTMLButtonElement;
export var ToastsWrapper = document.getElementById("toastsWrapper") as HTMLDivElement;

export function GetCurrentViewer(): HTMLElement {
    if (ScreenViewer.hasAttribute("hidden")) {
        return VideoScreenViewer;
    }
    return ScreenViewer;
}

export function Prompt(promptMessage: string): Promise<string> {
    return new Promise((resolve, reject) => {
        var modalDiv = document.createElement("div");
        modalDiv.classList.add("modal-prompt");

        var messageDiv = document.createElement("div");
        messageDiv.innerHTML = promptMessage;

        var responseInput = document.createElement("input");

        var buttonsDiv = document.createElement("div");
        buttonsDiv.classList.add("buttons-footer");

        var cancelButton = document.createElement("button");
        cancelButton.innerHTML = "Cancel";

        var okButton = document.createElement("button");
        okButton.innerHTML = "OK";

        buttonsDiv.appendChild(okButton);
        buttonsDiv.appendChild(cancelButton);
        modalDiv.appendChild(messageDiv);
        modalDiv.appendChild(responseInput);
        modalDiv.appendChild(buttonsDiv);

        document.body.appendChild(modalDiv);

        okButton.onclick = () => {
            modalDiv.remove();
            resolve(responseInput.value);
        }

        cancelButton.onclick = () => {
            modalDiv.remove();
            resolve(null);
        }
    });
}


export function SetScreenSize(width: number, height: number) {
    ScreenViewer.width = width;
    ScreenViewer.height = height;
    Screen2DContext.clearRect(0, 0, width, height);
}

export function ShowMessage(message: string) {
    var messageDiv = document.createElement("div");
    messageDiv.classList.add("toast-message");
    messageDiv.innerHTML = message;
    ToastsWrapper.appendChild(messageDiv);
    window.setTimeout(() => {
        messageDiv.remove();
    }, 5000);
}


export function ToggleConnectUI(shown: boolean) {
    if (shown) {
        Screen2DContext.clearRect(0, 0, ScreenViewer.width, ScreenViewer.height);
        ScreenViewer.setAttribute("hidden", "hidden");
        VideoScreenViewer.setAttribute("hidden", "hidden");
        if (ViewerApp.Mode == RemoteControlMode.Normal) {
            ConnectBox.style.removeProperty("display");
        }
        else {
            DisconnectedBox.style.removeProperty("display");
        }
        StreamVideoButton.classList.remove("toggled");
        BlockInputButton.classList.remove("toggled");
        AudioButton.classList.remove("toggled");
    }
    else {
        ConnectButton.removeAttribute("disabled");
        ConnectBox.style.display = "none";
        DisconnectedBox.style.display = "none";
        ScreenViewer.removeAttribute("hidden");
        StatusMessage.innerHTML = "";
    }
}

export function UpdateCursor(imageBytes: Uint8Array, hotSpotX: number, hotSpotY: number, cssOverride: string) {
    var targetElement = GetCurrentViewer();

    if (cssOverride) {
        targetElement.style.cursor = cssOverride;
    }
    else if (imageBytes.byteLength == 0) {
        targetElement.style.cursor = "default";
    }
    else {
        var base64 = ConvertUInt8ArrayToBase64(imageBytes);
        targetElement.style.cursor = `url('data:image/png;base64,${base64}') ${hotSpotX} ${hotSpotY}, default`;
    }
}

export function UpdateDisplays(selectedDisplay: string, displayNames: string[]) {
    ScreenSelectBar.innerHTML = "";
    for (let i = 0; i < displayNames.length; i++) {
        var button = document.createElement("button");
        button.innerHTML = `Monitor ${i}`;
        button.classList.add("horizontal-bar-button");
        if (displayNames[i] == selectedDisplay) {
            button.classList.add("toggled");
        }
        ScreenSelectBar.appendChild(button);
        button.onclick = (ev: MouseEvent) => {
            ViewerApp.MessageSender.SendSelectScreen(displayNames[i]);
            document.querySelectorAll("#screenSelectBar .horizontal-bar-button").forEach(button => {
                button.classList.remove("toggled");
            });
            (ev.currentTarget as HTMLButtonElement).classList.add("toggled");
        };
    }
}

export function UpdateStreamingToggled(toggleOn: boolean) {
    if (toggleOn) {
        StreamVideoButton.classList.add("toggled");
        VideoScreenViewer.removeAttribute("hidden");
        ScreenViewer.setAttribute("hidden", "hidden");
    }
    else {
        StreamVideoButton.classList.remove("toggled");
        ScreenViewer.removeAttribute("hidden");
        VideoScreenViewer.setAttribute("hidden", "hidden");
    }
}

export function UpdateWindowsSessions(windowsSessions: Array<WindowsSession>) {
    while (WindowsSessionSelect.options.length > 0) {
        WindowsSessionSelect.options.remove(0);
    }

    WindowsSessionSelect.options.add(document.createElement("option"));

    windowsSessions.forEach(x => {
        var sessionType = "";

        if (typeof x.Type == "number") {
            sessionType = x.Type == WindowsSessionType.Console ? "Console" : "RDP";
        }
        else {
            sessionType = x.Type;
        }

        var option = document.createElement("option");
        option.value = String(x.ID);
        option.text = `${sessionType} (ID: ${x.ID} | User: ${x.Username})`;
        option.title = `${sessionType} Session (ID: ${x.ID} | User: ${x.Username})`;
        WindowsSessionSelect.options.add(option);
    });
}