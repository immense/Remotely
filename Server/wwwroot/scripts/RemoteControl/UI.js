import { ConnectToClient, RemoteControl } from "./RemoteControl.js";
import { FloatMessage } from "../UI.js";
import { RemoteControlMode } from "../Enums/RemoteControlMode.js";
export var AudioButton = document.getElementById("audioButton");
export var MenuButton = document.getElementById("menuButton");
export var MenuFrame = document.getElementById("menuFrame");
export var SessionIDInput = document.getElementById("sessionIDInput");
export var ConnectButton = document.getElementById("connectButton");
export var RequesterNameInput = document.getElementById("nameInput");
export var StatusMessage = document.getElementById("statusMessage");
export var ScreenViewer = document.getElementById("screenViewer");
export var Screen2DContext = ScreenViewer.getContext("2d");
export var HorizontalBars = document.querySelectorAll(".horizontal-button-bar");
export var ConnectBox = document.getElementById("connectBox");
export var ScreenSelectBar = document.getElementById("screenSelectBar");
export var QualityBar = document.getElementById("qualityBar");
export var QualitySlider = document.getElementById("qualityRangeInput");
export var ActionsBar = document.getElementById("actionsBar");
export var ViewBar = document.getElementById("viewBar");
export var ChangeScreenButton = document.getElementById("changeScreenButton");
export var QualityButton = document.getElementById("qualityButton");
export var FitToScreenButton = document.getElementById("fitToScreenButton");
export var DisconnectButton = document.getElementById("disconnectButton");
export var OnScreenKeyboard = document.getElementById("osk");
export var FileTransferInput = document.getElementById("fileTransferInput");
export var FileTransferProgress = document.getElementById("fileTransferProgress");
export var KeyboardButton = document.getElementById("keyboardButton");
export var InviteButton = document.getElementById("inviteButton");
export var FileTransferButton = document.getElementById("fileTransferButton");
export var CtrlAltDelButton = document.getElementById("ctrlAltDelButton");
var lastPointerMove = Date.now();
var isDragging;
var currentPointerDevice;
var currentTouchCount;
var cancelNextClick;
var isPinchZooming;
var startPinchPoint1;
var startPinchPoint2;
var isMenuButtonDragging;
export function ApplyInputHandlers(sockets) {
    MenuButton.addEventListener("click", (ev) => {
        if (isMenuButtonDragging) {
            isMenuButtonDragging = false;
            ev.preventDefault();
            return;
        }
        MenuFrame.classList.toggle("open");
        MenuButton.classList.toggle("open");
        closeAllHorizontalBars(null);
    });
    MenuButton.addEventListener("mousemove", (ev) => {
        if (ev.buttons == 1) {
            ev.preventDefault();
            isMenuButtonDragging = true;
            MenuButton.style.top = `${ev.clientY}px`;
        }
    });
    MenuButton.addEventListener("touchmove", (ev) => {
        ev.preventDefault();
        isMenuButtonDragging = true;
        MenuButton.style.top = `${ev.touches[0].clientY}px`;
    });
    ChangeScreenButton.addEventListener("click", (ev) => {
        closeAllHorizontalBars("screenSelectBar");
        ScreenSelectBar.classList.toggle("open");
    });
    QualityButton.addEventListener("click", (ev) => {
        closeAllHorizontalBars("qualityBar");
        QualityBar.classList.toggle("open");
    });
    FitToScreenButton.addEventListener("click", (ev) => {
        var button = ev.currentTarget;
        button.classList.toggle("toggled");
        if (button.classList.contains("toggled")) {
            ScreenViewer.style.removeProperty("max-width");
            ScreenViewer.style.removeProperty("max-height");
        }
        else {
            ScreenViewer.style.maxWidth = "unset";
            ScreenViewer.style.maxHeight = "unset";
        }
    });
    AudioButton.addEventListener("click", (ev) => {
        AudioButton.classList.toggle("toggled");
        var toggleOn = AudioButton.classList.contains("toggled");
        sockets.SendToggleAudio(toggleOn);
    });
    DisconnectButton.addEventListener("click", (ev) => {
        ConnectButton.removeAttribute("disabled");
        RemoteControl.RCBrowserSockets.Connection.stop();
    });
    KeyboardButton.addEventListener("click", (ev) => {
        closeAllHorizontalBars(null);
        OnScreenKeyboard.classList.toggle("open");
    });
    InviteButton.addEventListener("click", (ev) => {
        var url = "";
        if (RemoteControl.Mode == RemoteControlMode.Normal) {
            url = `${location.origin}${location.pathname}?sessionID=${RemoteControl.ClientID}`;
        }
        else {
            url = `${location.origin}${location.pathname}?clientID=${RemoteControl.ClientID}&serviceID=${RemoteControl.ServiceID}`;
        }
        var input = document.createElement("input");
        input.style.position = "fixed";
        input.style.top = "-1000px";
        input.type = "text";
        document.body.appendChild(input);
        input.value = url;
        input.select();
        document.execCommand("copy", false, location.href);
        input.remove();
        FloatMessage("Link copied to clipboard.");
    });
    FileTransferButton.addEventListener("click", (ev) => {
        FileTransferInput.click();
    });
    FileTransferInput.addEventListener("change", (ev) => {
        uploadFiles(FileTransferInput.files);
    });
    CtrlAltDelButton.addEventListener("click", (ev) => {
        if (!RemoteControl.ServiceID) {
            ShowMessage("Not available for this session.");
            return;
        }
        closeAllHorizontalBars(null);
        RemoteControl.RCBrowserSockets.SendCtrlAltDel();
    });
    document.querySelectorAll("#sessionIDInput, #nameInput").forEach(x => {
        x.addEventListener("keypress", (ev) => {
            if (ev.key.toLowerCase() == "enter") {
                ConnectToClient();
            }
        });
    });
    ConnectButton.addEventListener("click", (ev) => {
        ConnectToClient();
    });
    QualitySlider.addEventListener("change", (ev) => {
        sockets.SendQualityChange(Number(QualitySlider.value));
    });
    ScreenViewer.addEventListener("pointermove", function (e) {
        currentPointerDevice = e.pointerType;
    });
    ScreenViewer.addEventListener("pointerdown", function (e) {
        currentPointerDevice = e.pointerType;
    });
    ScreenViewer.addEventListener("pointerenter", function (e) {
        currentPointerDevice = e.pointerType;
    });
    ScreenViewer.addEventListener("mousemove", function (e) {
        e.preventDefault();
        if (Date.now() - lastPointerMove < 25) {
            return;
        }
        lastPointerMove = Date.now();
        var percentX = e.offsetX / ScreenViewer.clientWidth;
        var percentY = e.offsetY / ScreenViewer.clientHeight;
        sockets.SendMouseMove(percentX, percentY);
    });
    ScreenViewer.addEventListener("mousedown", function (e) {
        if (currentPointerDevice == "touch") {
            return;
        }
        if (e.button != 0 && e.button != 2) {
            return;
        }
        e.preventDefault();
        var percentX = e.offsetX / ScreenViewer.clientWidth;
        var percentY = e.offsetY / ScreenViewer.clientHeight;
        sockets.SendMouseDown(e.button, percentX, percentY);
    });
    ScreenViewer.addEventListener("mouseup", function (e) {
        if (currentPointerDevice == "touch") {
            return;
        }
        if (e.button != 0 && e.button != 2) {
            return;
        }
        e.preventDefault();
        var percentX = e.offsetX / ScreenViewer.clientWidth;
        var percentY = e.offsetY / ScreenViewer.clientHeight;
        sockets.SendMouseUp(e.button, percentX, percentY);
    });
    ScreenViewer.addEventListener("click", function (e) {
        if (cancelNextClick) {
            cancelNextClick = false;
            return;
        }
        if (currentPointerDevice == "mouse") {
            e.preventDefault();
            e.stopPropagation();
        }
        else if (currentPointerDevice == "touch" && currentTouchCount == 0) {
            var percentX = e.offsetX / ScreenViewer.clientWidth;
            var percentY = e.offsetY / ScreenViewer.clientHeight;
            sockets.SendTap(percentX, percentY);
        }
    });
    ScreenViewer.addEventListener("dblclick", function (e) {
        if (currentPointerDevice == "mouse") {
            return;
        }
        var percentX = e.offsetX / ScreenViewer.clientWidth;
        var percentY = e.offsetY / ScreenViewer.clientHeight;
        sockets.SendMouseDown(2, percentX, percentY);
        sockets.SendMouseUp(2, percentX, percentY);
    });
    ScreenViewer.addEventListener("touchstart", function (e) {
        if (e.touches.length > 1) {
            cancelNextClick = true;
        }
        if (e.touches.length == 2) {
            startPinchPoint1 = { X: e.touches[0].pageX, Y: e.touches[0].pageY, IsEmpty: false };
            startPinchPoint2 = { X: e.touches[1].pageX, Y: e.touches[1].pageY, IsEmpty: false };
        }
        isDragging = false;
        currentTouchCount = e.touches.length;
        KeyboardButton.removeAttribute("hidden");
        var focusedInput = document.querySelector("input:focus");
        if (focusedInput) {
            focusedInput.blur();
        }
    });
    ScreenViewer.addEventListener("touchmove", function (e) {
        currentTouchCount = e.touches.length;
        var percentX = (e.touches[0].pageX - ScreenViewer.getBoundingClientRect().left) / ScreenViewer.clientWidth;
        var percentY = (e.touches[0].pageY - ScreenViewer.getBoundingClientRect().top) / ScreenViewer.clientHeight;
        if (e.touches.length == 2) {
            var distance1 = Math.hypot(startPinchPoint1.X - e.touches[0].pageX, startPinchPoint1.Y - e.touches[0].pageY);
            var distance2 = Math.hypot(startPinchPoint2.X - e.touches[1].pageX, startPinchPoint2.Y - e.touches[1].pageY);
            if (distance1 > 5 || distance2 > 5) {
                isPinchZooming = true;
            }
            return;
        }
        else if (isDragging) {
            e.preventDefault();
            e.stopPropagation();
            sockets.SendMouseMove(percentX, percentY);
        }
    });
    ScreenViewer.addEventListener("touchend", function (e) {
        currentTouchCount = e.touches.length;
        if (e.touches.length == 1 && !isPinchZooming) {
            isDragging = true;
            var percentX = (e.touches[0].pageX - ScreenViewer.getBoundingClientRect().left) / ScreenViewer.clientWidth;
            var percentY = (e.touches[0].pageY - ScreenViewer.getBoundingClientRect().top) / ScreenViewer.clientHeight;
            sockets.SendMouseMove(percentX, percentY);
            sockets.SendMouseDown(0, percentX, percentY);
            return;
        }
        if (currentTouchCount == 0) {
            cancelNextClick = false;
            isPinchZooming = false;
            startPinchPoint1 = null;
            startPinchPoint2 = null;
        }
        if (isDragging) {
            var percentX = (e.changedTouches[0].pageX - ScreenViewer.getBoundingClientRect().left) / ScreenViewer.clientWidth;
            var percentY = (e.changedTouches[0].pageY - ScreenViewer.getBoundingClientRect().top) / ScreenViewer.clientHeight;
            sockets.SendMouseUp(0, percentX, percentY);
        }
        isDragging = false;
    });
    ScreenViewer.addEventListener("contextmenu", (ev) => {
        ev.preventDefault();
    });
    ScreenViewer.addEventListener("wheel", function (e) {
        e.preventDefault();
        sockets.SendMouseWheel(e.deltaX, e.deltaY);
    });
    window.addEventListener("keydown", function (e) {
        if (document.querySelector("input:focus")) {
            return;
        }
        e.preventDefault();
        sockets.SendKeyDown(e.key);
    });
    window.addEventListener("keyup", function (e) {
        if (document.querySelector("input:focus")) {
            return;
        }
        e.preventDefault();
        sockets.SendKeyUp(e.key);
    });
    window.ondragover = function (e) {
        e.preventDefault();
        e.dataTransfer.dropEffect = "copy";
    };
    window.ondrop = function (e) {
        e.preventDefault();
        if (e.dataTransfer.files.length < 1) {
            return;
        }
        uploadFiles(e.dataTransfer.files);
    };
}
export function ShowMessage(message) {
    var messageDiv = document.createElement("div");
    messageDiv.classList.add("float-message");
    messageDiv.innerHTML = message;
    document.body.appendChild(messageDiv);
    window.setTimeout(() => {
        messageDiv.remove();
    }, 5000);
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
function uploadFiles(fileList) {
    ShowMessage("File upload started...");
    FileTransferProgress.value = 0;
    FileTransferProgress.parentElement.removeAttribute("hidden");
    var strPath = "/API/FileSharing/";
    var fd = new FormData();
    for (var i = 0; i < fileList.length; i++) {
        fd.append('fileUpload' + i, fileList[i]);
    }
    var xhr = new XMLHttpRequest();
    xhr.open('POST', strPath, true);
    xhr.addEventListener("load", function () {
        FileTransferProgress.parentElement.setAttribute("hidden", "hidden");
        if (xhr.status === 200) {
            ShowMessage("File upload completed.");
            RemoteControl.RCBrowserSockets.SendSharedFileIDs(xhr.responseText);
        }
        else {
            ShowMessage("File upload failed.");
        }
    });
    xhr.addEventListener("error", () => {
        FileTransferProgress.parentElement.setAttribute("hidden", "hidden");
        ShowMessage("File upload failed.");
    });
    xhr.addEventListener("progress", function (e) {
        FileTransferProgress.value = isFinite(e.loaded / e.total) ? e.loaded / e.total : 0;
    });
    xhr.send(fd);
}
function closeAllHorizontalBars(exceptBarId) {
    HorizontalBars.forEach(x => {
        if (x.id != exceptBarId) {
            x.classList.remove('open');
        }
    });
}
//# sourceMappingURL=UI.js.map