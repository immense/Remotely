import { RCBrowserSockets } from "./RCBrowserSockets.js";
import { GetDistanceBetween } from "../Utilities.js";
import { ConnectToClient, RemoteControl } from "./RemoteControl.js";
import { FloatMessage } from "../UI.js";
import { RemoteControlMode } from "../Enums/RemoteControlMode.js";

export var SessionIDInput = document.querySelector("#sessionIDInput") as HTMLInputElement;
export var ConnectButton = document.querySelector("#connectButton") as HTMLButtonElement;
export var RequesterNameInput = document.querySelector("#nameInput") as HTMLInputElement;
export var StatusMessage = document.querySelector("#statusMessage") as HTMLDivElement;
export var ScreenViewer = document.querySelector("#screenViewer") as HTMLCanvasElement;
export var Screen2DContext = ScreenViewer.getContext("2d");
export var HorizontalBars = document.querySelectorAll(".horizontal-button-bar");
export var ConnectBox = document.getElementById("connectBox") as HTMLDivElement;
export var ScreenSelectBar = document.querySelector("#screenSelectBar") as HTMLDivElement;
export var ConnectionBar = document.getElementById("connectionBar") as HTMLDivElement;
export var ActionsBar = document.getElementById("actionsBar") as HTMLDivElement;
export var OnScreenKeyboard = document.getElementById("osk") as HTMLDivElement;
export var FileTransferInput = document.getElementById("fileTransferInput") as HTMLInputElement;
export var FileTransferProgress = document.getElementById("fileTransferProgress") as HTMLProgressElement;
export var KeyboardButton = document.getElementById("keyboardButton") as HTMLButtonElement;

var lastPointerMove = Date.now();
var isDragging: boolean;
var currentPointerDevice: "Mouse" | "Touch";
var currentTouchCount: number;
var rightClickOpen: boolean;
var longPressTimer: number;
var cancelNextClick: boolean;

export function ApplyInputHandlers(sockets: RCBrowserSockets) {
    document.querySelector("#menuButton").addEventListener("click", (ev) => {
        HorizontalBars.forEach(x => {
            x.classList.remove('open');
        })
        ConnectionBar.classList.toggle("open");
    })
    document.querySelector("#actionsButton").addEventListener("click", (ev) => {
        HorizontalBars.forEach(x => {
            if (x.id != "actionsBar") {
                x.classList.remove('open');
            }
        })
        ActionsBar.classList.toggle("open");
    })
    document.querySelector("#changeScreenButton").addEventListener("click", (ev) => {
        HorizontalBars.forEach(x => {
            if (x.id != "screenSelectBar") {
                x.classList.remove('open');
            }
        })
        ScreenSelectBar.classList.toggle("open");
    })
    document.querySelector("#fitToScreenButton").addEventListener("click", (ev) => {
        var button = ev.currentTarget as HTMLButtonElement;
        button.classList.toggle("toggled");
        if (button.classList.contains("toggled")) {
            ScreenViewer.style.removeProperty("max-width");
            ScreenViewer.style.removeProperty("max-height");
        }
        else {
            ScreenViewer.style.maxWidth = "unset";
            ScreenViewer.style.maxHeight = "unset";
        }
    })
    document.querySelector("#disconnectButton").addEventListener("click", (ev) => {
        ConnectButton.removeAttribute("disabled");
        RemoteControl.RCBrowserSockets.Connection.stop();
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
        if (RemoteControl.Mode ==  RemoteControlMode.Normal) {
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
        
    })
    document.querySelector("#fileTransferButton").addEventListener("click", (ev) => {
        FileTransferInput.click();
    });
    (document.querySelector("#fileTransferInput") as HTMLInputElement).addEventListener("change", (ev) => {
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
    document.querySelector("#sessionIDInput, #nameInput").addEventListener("keypress", (ev: KeyboardEvent) => {
        if (ev.key.toLowerCase() == "enter") {
            ConnectToClient();
        }
    });
    document.querySelector("#connectButton").addEventListener("click", (ev) => {
        ConnectToClient();
    });
    ScreenViewer.addEventListener("mousemove", function (e) {
        currentPointerDevice = "Mouse";
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
        if (e.button != 0 && e.button != 2) {
            return;
        }
        e.preventDefault();
        var percentX = e.offsetX / ScreenViewer.clientWidth;
        var percentY = e.offsetY / ScreenViewer.clientHeight;
        sockets.SendMouseDown(e.button, percentX, percentY);
    });
    ScreenViewer.addEventListener("mouseup", function (e) {
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
        if (currentPointerDevice == "Mouse") {
            e.preventDefault();
            e.stopPropagation();
        }
        else if (currentPointerDevice == "Touch" && currentTouchCount == 0) {
            var percentX = e.offsetX / ScreenViewer.clientWidth;
            var percentY = e.offsetY / ScreenViewer.clientHeight;
            sockets.SendTap(percentX, percentY);
        }
    });
    ScreenViewer.addEventListener("dblclick", function (e) {
        var percentX = e.offsetX / ScreenViewer.clientWidth;
        var percentY = e.offsetY / ScreenViewer.clientHeight;
        sockets.SendMouseDown(2, percentX, percentY);
        sockets.SendMouseUp(2, percentX, percentY);
    });
    ScreenViewer.addEventListener("contextmenu", (ev) => {
        ev.preventDefault();
    });
    ScreenViewer.addEventListener("touchstart", function (e) {
        if (rightClickOpen) {
            e.preventDefault();
            e.stopPropagation();
            return;
        }
        if (e.touches.length > 1) {
            cancelNextClick = true;
        }
        isDragging = false;
        currentPointerDevice = "Touch";
        currentTouchCount = e.touches.length;
        KeyboardButton.removeAttribute("hidden");
        var focusedInput = document.querySelector("input:focus") as HTMLInputElement;
        if (focusedInput) {
            focusedInput.blur();
        }
    });

    ScreenViewer.addEventListener("touchmove", function (e) {
        if (rightClickOpen) {
            e.preventDefault();
            e.stopPropagation();
            return;
        }
        currentPointerDevice = "Touch";
        currentTouchCount = e.touches.length;
        var percentX = (e.touches[0].pageX - ScreenViewer.getBoundingClientRect().left) / ScreenViewer.clientWidth;
        var percentY = (e.touches[0].pageY - ScreenViewer.getBoundingClientRect().top) / ScreenViewer.clientHeight;

        if (e.touches.length == 2) {
            return;
        }
        else if (isDragging) {
            e.preventDefault();
            e.stopPropagation();
            sockets.SendMouseMove(percentX, percentY);
        }
    });
    ScreenViewer.addEventListener("touchend", function (e) {
        currentPointerDevice = "Touch";
        currentTouchCount = e.touches.length;

        if (e.touches.length == 1) {
            isDragging = true;
            var percentX = (e.touches[0].pageX - ScreenViewer.getBoundingClientRect().left) / ScreenViewer.clientWidth;
            var percentY = (e.touches[0].pageY - ScreenViewer.getBoundingClientRect().top) / ScreenViewer.clientHeight;
            sockets.SendMouseMove(percentX, percentY);
            sockets.SendMouseDown(0, percentX, percentY);
            return;
        }

        if (currentTouchCount == 0) {
            cancelNextClick = false;
            rightClickOpen = false;
        }


        if (isDragging) {
            var percentX = (e.changedTouches[0].pageX - ScreenViewer.getBoundingClientRect().left) / ScreenViewer.clientWidth;
            var percentY = (e.changedTouches[0].pageY - ScreenViewer.getBoundingClientRect().top) / ScreenViewer.clientHeight;
            sockets.SendMouseUp(0, percentX, percentY);
        }

        isDragging = false;

    });

    ScreenViewer.addEventListener("wheel", function (e) {
        e.preventDefault();
        sockets.SendMouseWheel(e.deltaX, e.deltaY);
    })
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

export function ShowMessage(message: string) {
    var messageDiv = document.createElement("div");
    messageDiv.classList.add("float-message");
    messageDiv.innerHTML = message;
    document.body.appendChild(messageDiv);
    window.setTimeout(() => {
        messageDiv.remove();
    }, 5000);
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

function uploadFiles(fileList: FileList) {
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