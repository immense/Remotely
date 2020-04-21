import * as UI from "./UI.js";
import { MainRc } from "./Main.js";
import { Sound } from "../Sound.js";
import { ShowMessage } from "../UI.js";
var signalR = window["signalR"];
export class RCBrowserSockets {
    constructor() {
        this.FpsStack = [];
    }
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
        MainRc.ClipboardWatcher.WatchClipboard();
    }
    ;
    SendIceCandidate(candidate) {
        if (candidate) {
            this.Connection.invoke("SendIceCandidateToAgent", candidate.candidate, candidate.sdpMLineIndex, candidate.sdpMid);
        }
        else {
            this.Connection.invoke("SendIceCandidateToAgent", "", 0, "");
        }
    }
    SendRtcAnswer(sessionDescription) {
        this.Connection.invoke("SendRtcAnswerToAgent", sessionDescription.sdp);
    }
    SendScreenCastRequestToDevice() {
        this.Connection.invoke("SendScreenCastRequestToDevice", MainRc.ClientID, MainRc.RequesterName, MainRc.Mode);
    }
    async SendFile(buffer, fileName, messageId, endOfFile, startOfFile) {
        await this.Connection.invoke("SendFile", buffer, fileName, messageId, endOfFile, startOfFile);
    }
    SendFrameReceived(bytesReceived) {
        this.Connection.invoke("SendFrameReceived", bytesReceived);
    }
    SendSelectScreen(displayName) {
        this.Connection.invoke("SelectScreen", displayName);
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
    SendToggleBlockInput(toggleOn) {
        this.Connection.invoke("SendToggleBlockInput", toggleOn);
    }
    SendClipboardTransfer(text, typeText) {
        this.Connection.invoke("SendClipboardTransfer", text, typeText);
    }
    ApplyMessageHandlers(hubConnection) {
        hubConnection.on("ClipboardTextChanged", (clipboardText) => {
            MainRc.ClipboardWatcher.SetClipboardText(clipboardText);
            ShowMessage("Clipboard updated.");
        });
        hubConnection.on("ScreenData", (selectedDisplay, displayNames) => {
            UI.UpdateDisplays(selectedDisplay, displayNames);
        });
        hubConnection.on("ScreenSize", (width, height) => {
            UI.SetScreenSize(width, height);
        });
        hubConnection.on("ScreenCapture", (buffer, left, top, width, height, imageQuality) => {
            this.SendFrameReceived(buffer.byteLength);
            if (UI.AutoQualityAdjustCheckBox.checked &&
                Number(UI.QualitySlider.value) != imageQuality) {
                UI.QualitySlider.value = String(imageQuality);
            }
            var url = window.URL.createObjectURL(new Blob([buffer]));
            var img = document.createElement("img");
            img.onload = () => {
                UI.Screen2DContext.drawImage(img, left, top, width, height);
                window.URL.revokeObjectURL(url);
            };
            img.src = url;
            if (MainRc.Debug) {
                this.FpsStack.push(Date.now());
                while (Date.now() - this.FpsStack[0] > 1000) {
                    this.FpsStack.shift();
                }
                console.log("FPS: " + String(this.FpsStack.length));
            }
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
            MainRc.ClientID = newClientID;
            this.Connection.stop();
            this.Connect();
        });
        hubConnection.on("Reconnecting", () => {
            UI.ShowMessage("Reconnecting...");
        });
        hubConnection.on("CursorChange", (cursor) => {
            UI.UpdateCursor(cursor.ImageBytes, cursor.HotSpot.X, cursor.HotSpot.Y, cursor.CssOverride);
        });
        hubConnection.on("RequestingScreenCast", () => {
            UI.ShowMessage("Requesting remote control...");
        });
        hubConnection.on("ReceiveRtcOffer", async (sdp) => {
            console.log("Rtc offer SDP received.");
            MainRc.RtcSession.Init();
            await MainRc.RtcSession.ReceiveRtcOffer(sdp);
        });
        hubConnection.on("ReceiveIceCandidate", (candidate, sdpMlineIndex, sdpMid) => {
            console.log("Ice candidate received.");
            MainRc.RtcSession.ReceiveCandidate({
                candidate: candidate,
                sdpMLineIndex: sdpMlineIndex,
                sdpMid: sdpMid
            });
        });
        hubConnection.on("ShowMessage", (message) => {
            UI.ShowMessage(message);
        });
    }
}
//# sourceMappingURL=RCBrowserSockets.js.map