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
export var ScreenViewerWrapper = document.getElementById("screenViewerWrapper");
export var Screen2DContext = ScreenViewer ? ScreenViewer.getContext("2d") : null;
export var PopupMenus = document.querySelectorAll(".popup-menu");
export var ConnectBox = document.getElementById("connectBox");
export var ConnectHeader = document.getElementById("connectHeader");
export var ConnectForm = document.getElementById("connectForm");
export var ScreenSelectMenu = document.getElementById("screenSelectMenu");
export var ChangeScreenButton = document.getElementById("changeScreenButton");
export var FitToScreenButton = document.getElementById("fitToScreenButton");
export var BlockInputButton = document.getElementById("blockInputButton");
export var DisconnectButton = document.getElementById("disconnectButton");
export var FileTransferInput = document.getElementById("fileTransferInput");
export var FileTransferProgress = document.getElementById("fileTransferProgress");
export var FileTransferNameSpan = document.getElementById("fileTransferNameSpan");
export var KeyboardButton = document.getElementById("keyboardButton");
export var InviteButton = document.getElementById("inviteButton");
export var FileTransferButton = document.getElementById("fileTransferButton");
export var FileTransferMenu = document.getElementById("fileTransferMenu");
export var FileUploadButtton = document.getElementById("fileUploadButton");
export var FileDownloadButton = document.getElementById("fileDownloadButton");
export var CtrlAltDelButton = document.getElementById("ctrlAltDelButton");
export var TouchKeyboardInput = document.getElementById("touchKeyboardInput");
export var ClipboardTransferMenu = document.getElementById("clipboardTransferMenu");
export var ClipboardTransferButton = document.getElementById("clipboardTransferButton");
export var TypeClipboardButton = document.getElementById("typeClipboardButton");
export var ConnectionP2PIcon = document.getElementById("connectionP2PIcon");
export var ConnectionRelayedIcon = document.getElementById("connectionRelayedIcon");
export var WindowsSessionSelect = document.getElementById("windowsSessionSelect");
export var ViewOnlyButton = document.getElementById("viewOnlyButton");
export var FullScreenButton = document.getElementById("fullScreenButton");
export var ToastsWrapper = document.getElementById("toastsWrapper");
export var MbpsDiv = document.getElementById("mbpsDiv");
export var FpsDiv = document.getElementById("fpsDiv");
export var LatencyDiv = document.getElementById("latencyDiv");
export var GpuDiv = document.getElementById("gpuAcceleratedDiv");
export var WorkAreaGrid = document.getElementById("workAreaGrid");
export var BackgroundLayers = document.getElementById("backgroundLayers");
export var ExtrasMenu = document.getElementById("extrasMenu");
export var ExtrasMenuButton = document.getElementById("extrasMenuButton");
export var WindowsSessionMenu = document.getElementById("windowsSessionMenu");
export var WindowsSessionMenuButton = document.getElementById("windowsSessionMenuButton");
export var MetricsButton = document.getElementById("metricsButton");
export var MetricsFrame = document.getElementById("metricsFrame");
export var BetaPillPullDown = document.getElementById("betaPillPullDown");
export function CloseAllPopupMenus(exceptMenuId) {
    PopupMenus.forEach(x => {
        if (x.id != exceptMenuId) {
            x.classList.remove("open");
        }
    });
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
export function SetStatusMessage(message) {
    StatusMessage.innerText = message;
}
export function ShowToast(message) {
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
        ConnectButton.innerText = "Connect";
        Screen2DContext.clearRect(0, 0, ScreenViewer.width, ScreenViewer.height);
        ScreenViewerWrapper.setAttribute("hidden", "hidden");
        if (ViewerApp.Mode == RemoteControlMode.Attended) {
            ConnectBox.style.removeProperty("display");
            ConnectHeader.style.removeProperty("display");
        }
        BlockInputButton.classList.remove("toggled");
        AudioButton.classList.remove("toggled");
        WorkAreaGrid.style.display = "none";
        BackgroundLayers.classList.remove("d-none");
        CloseAllPopupMenus(null);
    }
    else {
        ConnectBox.style.display = "none";
        ConnectHeader.style.display = "none";
        ScreenViewerWrapper.removeAttribute("hidden");
        StatusMessage.innerHTML = "";
        WorkAreaGrid.style.removeProperty("display");
        BackgroundLayers.classList.add("d-none");
    }
    ConnectButton.disabled = !ViewerApp.RequesterName || !ViewerApp.SessionId;
}
export function UpdateCursor(imageBytes, hotSpotX, hotSpotY, cssOverride) {
    if (cssOverride) {
        ScreenViewer.style.cursor = cssOverride;
    }
    else if (imageBytes.byteLength == 0) {
        ScreenViewer.style.cursor = "default";
    }
    else {
        var base64 = ConvertUInt8ArrayToBase64(imageBytes);
        ScreenViewer.style.cursor = `url('data:image/png;base64,${base64}') ${hotSpotX} ${hotSpotY}, default`;
    }
}
export function UpdateDisplays(selectedDisplay, displayNames) {
    ScreenSelectMenu.innerHTML = "";
    for (let i = 0; i < displayNames.length; i++) {
        var button = document.createElement("button");
        button.innerHTML = `Monitor ${i}`;
        if (displayNames[i] == selectedDisplay) {
            button.classList.add("toggled");
        }
        ScreenSelectMenu.appendChild(button);
        button.onclick = (ev) => {
            ViewerApp.MessageSender.SendSelectScreen(displayNames[i]);
            ScreenSelectMenu.classList.toggle("open");
            ScreenSelectMenu.querySelectorAll("button").forEach(button => {
                button.classList.remove("toggled");
            });
            ev.currentTarget.classList.add("toggled");
        };
    }
}
export function UpdateMetrics(metricsDto) {
    FpsDiv.innerHTML = metricsDto.Fps.toFixed(0);
    MbpsDiv.innerHTML = metricsDto.Mbps.toFixed(2);
    LatencyDiv.innerHTML = `${metricsDto.RoundTripLatency.toFixed(2)}ms`;
    GpuDiv.innerHTML = metricsDto.IsGpuAccelerated ? "Enabled" : "Unavailable";
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