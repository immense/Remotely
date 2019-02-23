import { GetDistanceBetween } from "../Utilities.js";
import { ConnectToClient, RemoteControl } from "./RemoteControl.js";
import { FloatMessage } from "../UI.js";
export var SessionIDInput = document.querySelector("#sessionIDInput");
export var ConnectButton = document.querySelector("#connectButton");
export var RequesterNameInput = document.querySelector("#nameInput");
export var StatusMessage = document.querySelector("#statusMessage");
export var ScreenViewer = document.querySelector("#screenViewer");
export var HorizontalBars = document.querySelectorAll(".horizontal-button-bar");
export var ConnectBox = document.getElementById("connectBox");
export var ScreenSelectBar = document.querySelector("#screenSelectBar");
export var ConnectionBar = document.getElementById("connectionBar");
export var ActionsBar = document.getElementById("actionsBar");
export var OnScreenKeyboard = document.getElementById("osk");
export var FileTransferInput = document.getElementById("fileTransferInput");
export var FileTransferProgress = document.getElementById("fileTransferProgress");
export var KeyboardButton = document.getElementById("keyboardButton");
var lastPointerMove = Date.now();
var lastTouchPointX;
var lastTouchPointY;
var lastTouchStart = Date.now();
var touchList = new Array();
var longPressTimeout;
var lastTouchDistanceMoved = 0;
export function ApplyInputHandlers(sockets, rtc) {
    document.querySelector("#menuButton").addEventListener("click", (ev) => {
        HorizontalBars.forEach(x => {
            x.classList.remove('open');
        });
        ConnectionBar.classList.toggle("open");
    });
    document.querySelector("#actionsButton").addEventListener("click", (ev) => {
        HorizontalBars.forEach(x => {
            if (x.id != "actionsBar") {
                x.classList.remove('open');
            }
        });
        ActionsBar.classList.toggle("open");
    });
    document.querySelector("#changeScreenButton").addEventListener("click", (ev) => {
        HorizontalBars.forEach(x => {
            if (x.id != "screenSelectBar") {
                x.classList.remove('open');
            }
        });
        ScreenSelectBar.classList.toggle("open");
    });
    document.querySelector("#fitToScreenButton").addEventListener("click", (ev) => {
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
    document.querySelector("#disconnectButton").addEventListener("click", (ev) => {
        rtc.Disconnect();
        ConnectButton.removeAttribute("disabled");
    });
    document.querySelector("#keyboardButton").addEventListener("click", (ev) => {
        HorizontalBars.forEach(x => {
            x.classList.remove('open');
        });
        ConnectionBar.classList.remove("open");
        OnScreenKeyboard.classList.toggle("open");
    });
    document.querySelector("#inviteButton").addEventListener("click", (ev) => {
        var url = "";
        if (RemoteControl.Mode == "Normal") {
            url = `${location.origin}${location.pathname}?sessionID=${RemoteControl.ClientID}`;
        }
        else {
            url = location.href;
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
    document.querySelector("#fileTransferButton").addEventListener("click", (ev) => {
        FileTransferInput.click();
    });
    document.querySelector("#fileTransferInput").addEventListener("change", (ev) => {
        uploadFiles(FileTransferInput.files);
    });
    document.querySelector("#ctrlAltDelButton").addEventListener("click", (ev) => {
        if (!RemoteControl.ServiceID) {
            ShowMessage("Not available for this session.");
            return;
        }
        HorizontalBars.forEach(x => {
            x.classList.remove('open');
        });
        ConnectionBar.classList.remove("open");
        RemoteControl.RCBrowserSockets.SendCtrlAltDel();
    });
    document.querySelector("#sessionIDInput, #nameInput").addEventListener("keypress", (ev) => {
        if (ev.key.toLowerCase() == "enter") {
            ConnectToClient();
        }
    });
    document.querySelector("#connectButton").addEventListener("click", (ev) => {
        ConnectToClient();
    });
    ScreenViewer.addEventListener("mousemove", function (e) {
        e.preventDefault();
        if (Date.now() - lastPointerMove < 50) {
            return;
        }
        lastPointerMove = Date.now();
        var percentX = e.offsetX / ScreenViewer.clientWidth;
        var percentY = e.offsetY / ScreenViewer.clientHeight;
        sockets.SendMouseMove(percentX, percentY);
    });
    ScreenViewer.addEventListener("mousedown", function (e) {
        if (e.button != 0 && e.button != 2) {
            return;
        }
        e.preventDefault();
        var percentX = e.offsetX / ScreenViewer.clientWidth;
        var percentY = e.offsetY / ScreenViewer.clientHeight;
        var button;
        if (e.button == 0) {
            button = "left";
        }
        else if (e.button == 2) {
            button = "right";
        }
        sockets.SendMouseDown(button, percentX, percentY);
    });
    ScreenViewer.addEventListener("mouseup", function (e) {
        if (e.button != 0 && e.button != 2) {
            return;
        }
        e.preventDefault();
        var percentX = e.offsetX / ScreenViewer.clientWidth;
        var percentY = e.offsetY / ScreenViewer.clientHeight;
        var button;
        if (e.button == 0) {
            button = "left";
        }
        else if (e.button == 2) {
            button = "right";
        }
        sockets.SendMouseUp(button, percentX, percentY);
    });
    ScreenViewer.addEventListener("click", function (e) {
        e.preventDefault();
        e.stopPropagation();
    });
    ScreenViewer.addEventListener("contextmenu", (ev) => {
        ev.preventDefault();
    });
    ScreenViewer.addEventListener("touchstart", function (e) {
        KeyboardButton.removeAttribute("hidden");
        var focusedInput = document.querySelector("input:focus");
        if (focusedInput) {
            focusedInput.blur();
        }
        touchList.push(e.changedTouches[0].identifier);
        if (e.touches.length > 1) {
            window.clearTimeout(longPressTimeout);
            return;
        }
        e.preventDefault();
        e.stopPropagation();
        if (Date.now() - lastTouchStart < 500) {
            sockets.SendTouchDown();
            return;
        }
        lastTouchStart = Date.now();
        lastTouchPointX = e.touches[0].clientX;
        lastTouchPointY = e.touches[0].clientY;
        lastTouchDistanceMoved = 0;
        longPressTimeout = window.setTimeout(() => {
            if (lastTouchStart > lastPointerMove && touchList.some(x => x == e.changedTouches[0].identifier)) {
                sockets.SendLongPress();
            }
        }, 1500);
    });
    ScreenViewer.addEventListener("touchmove", function (e) {
        if (e.touches.length > 1) {
            return;
        }
        e.preventDefault();
        e.stopPropagation();
        if (Date.now() - lastPointerMove < 50) {
            return;
        }
        lastTouchDistanceMoved = GetDistanceBetween(lastTouchPointX, lastTouchPointY, e.touches[0].clientX, e.touches[0].clientY);
        var moveX = (e.touches[0].clientX - lastTouchPointX) * 2;
        var moveY = (e.touches[0].clientY - lastTouchPointY) * 2;
        sockets.SendTouchMove(moveX, moveY);
        lastTouchPointX = e.touches[0].clientX;
        lastTouchPointY = e.touches[0].clientY;
        lastPointerMove = Date.now();
    });
    ScreenViewer.addEventListener("touchend", function (e) {
        var index = touchList.findIndex(x => x == e.changedTouches[0].identifier);
        touchList.splice(index, 1);
        e.preventDefault();
        e.stopPropagation();
        if (e.touches.length > 0) {
            return;
        }
        if (Date.now() - lastTouchStart < 500 && lastTouchDistanceMoved < 5) {
            sockets.SendTap();
        }
        else {
            sockets.SendTouchUp();
        }
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
        var key = e.key.toLowerCase();
        sockets.SendKeyDown(key);
    });
    window.addEventListener("keyup", function (e) {
        if (document.querySelector("input:focus")) {
            return;
        }
        e.preventDefault();
        var key = e.key.toLowerCase();
        sockets.SendKeyUp(key);
    });
    window.addEventListener("click", function (e) {
        ScreenViewer.muted = false;
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
//# sourceMappingURL=UI.js.map