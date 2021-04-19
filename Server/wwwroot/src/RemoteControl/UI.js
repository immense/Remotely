import { ViewerApp } from "./App.js";
import { ConvertUInt8ArrayToBase64 } from "./Utilities.js";
import { WindowsSessionType } from "./Enums/WindowsSessionType.js";
import { RemoteControlMode } from "./Enums/RemoteControlMode.js";
export var AudioButton = document.getElementById("audioButton");
export var MenuButton = document.getElementById("menuButton");
export var MenuFrame = document.getElementById("menuFrame");
export var SessionIDInput = document.getElementById("sessionIDInput");
export var ConnectButton = document.getElementById("connectButton");
export var RequesterNameInput = document.getElementById("nameInput");
export var StatusMessage = document.getElementById("statusMessage");
export var ScreenViewer = document.getElementById("screenViewer");
export var VideoScreenViewer = document.getElementById("videoScreenViewer");
export var ScreenViewerWrapper = document.getElementById("screenViewerWrapper");
export var Screen2DContext = ScreenViewer ? ScreenViewer.getContext("2d") : null;
export var HorizontalBars = document.querySelectorAll(".horizontal-button-bar");
export var ConnectBox = document.getElementById("connectBox");
export var DisconnectedBox = document.getElementById("disconnectedBox");
export var ScreenSelectBar = document.getElementById("screenSelectBar");
export var ActionsBar = document.getElementById("actionsBar");
export var ViewBar = document.getElementById("viewBar");
export var ChangeScreenButton = document.getElementById("changeScreenButton");
export var StreamVideoButton = document.getElementById("streamVideoButton");
export var FitToScreenButton = document.getElementById("fitToScreenButton");
export var BlockInputButton = document.getElementById("blockInputButton");
export var DisconnectButton = document.getElementById("disconnectButton");
export var FileTransferInput = document.getElementById("fileTransferInput");
export var FileTransferProgress = document.getElementById("fileTransferProgress");
export var FileTransferNameSpan = document.getElementById("fileTransferNameSpan");
export var KeyboardButton = document.getElementById("keyboardButton");
export var InviteButton = document.getElementById("inviteButton");
export var FileTransferButton = document.getElementById("fileTransferButton");
export var FileTransferBar = document.getElementById("fileTransferBar");
export var FileUploadButtton = document.getElementById("fileUploadButton");
export var FileDownloadButton = document.getElementById("fileDownloadButton");
export var CtrlAltDelButton = document.getElementById("ctrlAltDelButton");
export var TouchKeyboardTextArea = document.getElementById("touchKeyboardTextArea");
export var ClipboardTransferBar = document.getElementById("clipboardTransferBar");
export var ClipboardTransferButton = document.getElementById("clipboardTransferButton");
export var TypeClipboardButton = document.getElementById("typeClipboardButton");
export var ConnectionP2PIcon = document.getElementById("connectionP2PIcon");
export var ConnectionRelayedIcon = document.getElementById("connectionRelayedIcon");
export var WindowsSessionSelect = document.getElementById("windowsSessionSelect");
export var RecordSessionButton = document.getElementById("recordSessionButton");
export var DownloadRecordingButton = document.getElementById("downloadRecordingButton");
export var ViewOnlyButton = document.getElementById("viewOnlyButton");
export var FullScreenButton = document.getElementById("fullScreenButton");
export var ToastsWrapper = document.getElementById("toastsWrapper");
export function GetCurrentViewer() {
    if (ScreenViewer.hasAttribute("hidden")) {
        return VideoScreenViewer;
    }
    return ScreenViewer;
}
export function Prompt(promptMessage) {
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
        };
        cancelButton.onclick = () => {
            modalDiv.remove();
            resolve(null);
        };
    });
}
export function SetScreenSize(width, height) {
    ScreenViewer.width = width;
    ScreenViewer.height = height;
    Screen2DContext.clearRect(0, 0, width, height);
}
export function ShowMessage(message) {
    var messageDiv = document.createElement("div");
    messageDiv.classList.add("toast-message");
    messageDiv.innerHTML = message;
    ToastsWrapper.appendChild(messageDiv);
    window.setTimeout(() => {
        messageDiv.remove();
    }, 5000);
}
export function ToggleConnectUI(shown) {
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
export function UpdateCursor(imageBytes, hotSpotX, hotSpotY, cssOverride) {
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
export function UpdateDisplays(selectedDisplay, displayNames) {
    ScreenSelectBar.innerHTML = "";
    for (let i = 0; i < displayNames.length; i++) {
        var button = document.createElement("button");
        button.innerHTML = `Monitor ${i}`;
        button.classList.add("horizontal-bar-button");
        if (displayNames[i] == selectedDisplay) {
            button.classList.add("toggled");
        }
        ScreenSelectBar.appendChild(button);
        button.onclick = (ev) => {
            ViewerApp.MessageSender.SendSelectScreen(displayNames[i]);
            document.querySelectorAll("#screenSelectBar .horizontal-bar-button").forEach(button => {
                button.classList.remove("toggled");
            });
            ev.currentTarget.classList.add("toggled");
        };
    }
}
export function UpdateStreamingToggled(toggleOn) {
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
export function UpdateWindowsSessions(windowsSessions) {
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
//# sourceMappingURL=UI.js.map