import * as Utilities from "../Utilities.js";
import * as UI from "./UI.js";
import { RemoteControl } from "./Main.js";
import { Sound } from "../Sound.js";
import { PopupMessage } from "../UI.js";
var signalR = window["signalR"];
export class RCBrowserSockets {
    Connect() {
        this.Connection = new signalR.HubConnectionBuilder()
            .withUrl("/RCBrowserHub")
            .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
            .configureLogging(signalR.LogLevel.Information)
            .build();
        this.ApplyMessageHandlers(this.Connection);
        this.Connection.start().then(() => {
            this.SendScreenCastRequestToDevice();
            UI.ConnectButton.removeAttribute("disabled");
            UI.ConnectBox.style.display = "none";
            UI.ScreenViewer.removeAttribute("hidden");
            UI.StatusMessage.innerHTML = "";
        }).catch(err => {
            console.error(err.toString());
            console.log("Connection closed.");
            UI.StatusMessage.innerHTML = `Connection error: ${err.message}`;
            UI.Screen2DContext.clearRect(0, 0, UI.ScreenViewer.width, UI.ScreenViewer.height);
            UI.ScreenViewer.setAttribute("hidden", "hidden");
            UI.ConnectBox.style.removeProperty("display");
        });
        this.Connection.closedCallbacks.push((ev) => {
            UI.Screen2DContext.clearRect(0, 0, UI.ScreenViewer.width, UI.ScreenViewer.height);
            UI.ScreenViewer.setAttribute("hidden", "hidden");
            UI.ConnectBox.style.removeProperty("display");
        });
    }
    ;
    SendScreenCastRequestToDevice() {
        this.Connection.invoke("SendScreenCastRequestToDevice", RemoteControl.ClientID, RemoteControl.RequesterName, RemoteControl.Mode);
    }
    SendLatencyUpdate(sentTime, bytesReceived) {
        this.Connection.invoke("SendLatencyUpdate", sentTime, bytesReceived);
    }
    SendSelectScreen(index) {
        this.Connection.invoke("SelectScreen", index);
    }
    SendMouseMove(percentX, percentY) {
        this.Connection.invoke("MouseMove", percentX, percentY);
    }
    SendMouseDown(button, percentX, percentY) {
        this.Connection.invoke("MouseDown", button, percentX, percentY);
    }
    SendMouseUp(button, percentX, percentY) {
        this.Connection.invoke("MouseUp", button, percentX, percentY);
    }
    SendTouchDown() {
        this.Connection.invoke("TouchDown");
    }
    SendLongPress() {
        this.Connection.invoke("LongPress");
    }
    SendTouchMove(moveX, moveY) {
        this.Connection.invoke("TouchMove", moveX, moveY);
    }
    SendTouchUp() {
        this.Connection.invoke("TouchUp");
    }
    SendTap(percentX, percentY) {
        this.Connection.invoke("Tap", percentX, percentY);
    }
    SendMouseWheel(deltaX, deltaY) {
        this.Connection.invoke("MouseWheel", deltaX, deltaY);
    }
    SendKeyDown(key) {
        this.Connection.invoke("KeyDown", key);
    }
    SendKeyUp(key) {
        this.Connection.invoke("KeyUp", key);
    }
    SendKeyPress(key) {
        this.Connection.invoke("KeyPress", key);
    }
    SendCtrlAltDel() {
        this.Connection.invoke("CtrlAltDel");
    }
    SendSharedFileIDs(fileIDs) {
        this.Connection.invoke("SendSharedFileIDs", JSON.parse(fileIDs));
    }
    SendQualityChange(qualityLevel) {
        this.Connection.invoke("SendQualityChange", qualityLevel);
    }
    SendAutoQualityAdjust(isOn) {
        this.Connection.invoke("SendAutoQualityAdjust", isOn);
    }
    SendToggleAudio(toggleOn) {
        this.Connection.invoke("SendToggleAudio", toggleOn);
    }
    ;
    SendClipboardTransfer(text, typeText) {
        this.Connection.invoke("SendClipboardTransfer", text, typeText);
    }
    ApplyMessageHandlers(hubConnection) {
        hubConnection.on("ClipboardTextChanged", (clipboardText) => {
            Utilities.SetClipboardText(clipboardText);
            UI.ClipboardTransferTextArea.value = clipboardText;
            PopupMessage("Clipboard updated.");
        });
        hubConnection.on("ScreenCount", (primaryScreenIndex, screenCount) => {
            document.querySelector("#screenSelectBar").innerHTML = "";
            for (let i = 0; i < screenCount; i++) {
                var button = document.createElement("button");
                button.innerHTML = `Monitor ${i}`;
                button.classList.add("horizontal-bar-button");
                if (i == primaryScreenIndex) {
                    button.classList.add("toggled");
                }
                document.querySelector("#screenSelectBar").appendChild(button);
                button.onclick = (ev) => {
                    this.SendSelectScreen(i);
                    document.querySelectorAll("#screenSelectBar .horizontal-bar-button").forEach(button => {
                        button.classList.remove("toggled");
                    });
                    ev.currentTarget.classList.add("toggled");
                };
            }
        });
        hubConnection.on("ScreenSize", (width, height) => {
            UI.ScreenViewer.width = width;
            UI.ScreenViewer.height = height;
            UI.Screen2DContext.clearRect(0, 0, width, height);
        });
        hubConnection.on("ScreenCapture", (buffer, left, top, width, height, captureTime) => {
            this.SendLatencyUpdate(captureTime, buffer.byteLength);
            var url = window.URL.createObjectURL(new Blob([buffer]));
            var img = document.createElement("img");
            img.onload = () => {
                UI.Screen2DContext.drawImage(img, left, top, width, height);
                window.URL.revokeObjectURL(url);
            };
            img.src = url;
        });
        hubConnection.on("AudioSample", (buffer) => {
            Sound.Play(buffer);
        });
        hubConnection.on("ConnectionFailed", () => {
            UI.ConnectButton.removeAttribute("disabled");
            UI.StatusMessage.innerHTML = "Connection failed or was denied.";
            UI.ShowMessage("Connection failed.  Please reconnect.");
            this.Connection.stop();
        });
        hubConnection.on("ViewerRemoved", () => {
            UI.ConnectButton.removeAttribute("disabled");
            UI.StatusMessage.innerHTML = "The session was stopped by your partner.";
            UI.ShowMessage("Session ended.");
            this.Connection.stop();
        });
        hubConnection.on("SessionIDNotFound", () => {
            UI.ConnectButton.removeAttribute("disabled");
            UI.StatusMessage.innerHTML = "Session ID not found.";
            this.Connection.stop();
        });
        hubConnection.on("ScreenCasterDisconnected", () => {
            UI.StatusMessage.innerHTML = "The host has disconnected.";
            this.Connection.stop();
        });
        hubConnection.on("ReceiveMachineName", (machineName) => {
            document.title = `${machineName} - Remotely Session`;
        });
        hubConnection.on("RelaunchedScreenCasterReady", (newClientID) => {
            RemoteControl.ClientID = newClientID;
            this.Connection.stop();
            this.Connect();
        });
        hubConnection.on("Reconnecting", () => {
            UI.ShowMessage("Reconnecting...");
        });
        hubConnection.on("CursorChange", (cursor) => {
            if (cursor.CssOverride) {
                UI.ScreenViewer.style.cursor = cursor.CssOverride;
            }
            else if (cursor.ImageBytes.byteLength == 0) {
                UI.ScreenViewer.style.cursor = "default";
            }
            else {
                var base64 = Utilities.ConvertUInt8ArrayToBase64(cursor.ImageBytes);
                UI.ScreenViewer.style.cursor = `url('data:image/png;base64,${base64}') ${cursor.HotSpot.X} ${cursor.HotSpot.Y}, default`;
            }
        });
        hubConnection.on("RequestingScreenCast", () => {
            UI.ShowMessage("Requesting remote control...");
        });
    }
}
//# sourceMappingURL=RCBrowserSockets.js.map