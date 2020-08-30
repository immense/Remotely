import * as UI from "./UI.js";
import { MainViewer } from "./Main.js";
import { Sound } from "../Sound.js";
import { ShowMessage } from "../UI.js";
import { RemoteControlMode } from "../Enums/RemoteControlMode.js";
var signalR = window["signalR"];
export class ViewerHubConnection {
    constructor() {
        this.PartialCaptureFrames = [];
    }
    Connect() {
        this.Connection = new signalR.HubConnectionBuilder()
            .withUrl("/ViewerHub")
            .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
            .configureLogging(signalR.LogLevel.Information)
            .build();
        this.ApplyMessageHandlers(this.Connection);
        this.Connection.start().then(() => {
            this.SendScreenCastRequestToDevice();
            this.ToggleConnectUI(false);
        }).catch(err => {
            console.error(err.toString());
            console.log("Connection closed.");
            UI.StatusMessage.innerHTML = `Connection error: ${err.message}`;
            this.ToggleConnectUI(true);
        });
        this.Connection.closedCallbacks.push((ev) => {
            this.ToggleConnectUI(true);
        });
        MainViewer.ClipboardWatcher.WatchClipboard();
    }
    GetWindowsSessions() {
        if (MainViewer.Mode == RemoteControlMode.Unattended) {
            this.Connection.invoke("GetWindowsSessions");
        }
    }
    ChangeWindowsSession(sessionID) {
        if (MainViewer.Mode == RemoteControlMode.Unattended) {
            this.Connection.invoke("ChangeWindowsSession", sessionID);
        }
    }
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
        this.Connection.invoke("SendScreenCastRequestToDevice", MainViewer.ClientID, MainViewer.RequesterName, MainViewer.Mode, MainViewer.Otp);
    }
    async SendFile(buffer, fileName, messageId, endOfFile, startOfFile) {
        await this.Connection.invoke("SendFile", buffer, fileName, messageId, endOfFile, startOfFile);
    }
    SendFrameReceived() {
        this.Connection.invoke("SendFrameReceived");
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
    SendSetKeyStatesUp() {
        this.Connection.invoke("SendSetKeyStatesUp");
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
    SendToggleWebRtcVideo(toggleOn) {
        this.Connection.invoke("SendToggleWebRtcVideo", toggleOn);
    }
    SendClipboardTransfer(text, typeText) {
        this.Connection.invoke("SendClipboardTransfer", text, typeText);
    }
    ToggleConnectUI(shown) {
        if (shown) {
            UI.Screen2DContext.clearRect(0, 0, UI.ScreenViewer.width, UI.ScreenViewer.height);
            UI.ScreenViewer.setAttribute("hidden", "hidden");
            UI.VideoScreenViewer.setAttribute("hidden", "hidden");
            UI.ConnectBox.style.removeProperty("display");
            UI.StreamVideoButton.classList.remove("toggled");
            UI.BlockInputButton.classList.remove("toggled");
            UI.AudioButton.classList.remove("toggled");
        }
        else {
            UI.ConnectButton.removeAttribute("disabled");
            UI.ConnectBox.style.display = "none";
            UI.ScreenViewer.removeAttribute("hidden");
            UI.StatusMessage.innerHTML = "";
        }
    }
    ApplyMessageHandlers(hubConnection) {
        hubConnection.on("ClipboardTextChanged", (clipboardText) => {
            MainViewer.ClipboardWatcher.SetClipboardText(clipboardText);
            ShowMessage("Clipboard updated.");
        });
        hubConnection.on("ScreenData", (selectedDisplay, displayNames) => {
            UI.UpdateDisplays(selectedDisplay, displayNames);
        });
        hubConnection.on("ScreenSize", (width, height) => {
            UI.SetScreenSize(width, height);
        });
        hubConnection.on("ScreenCapture", (buffer, left, top, width, height, imageQuality, endOfFrame) => {
            if (UI.AutoQualityAdjustCheckBox.checked && Number(UI.QualitySlider.value) != imageQuality) {
                UI.QualitySlider.value = String(imageQuality);
            }
            if (endOfFrame) {
                this.SendFrameReceived();
                var url = window.URL.createObjectURL(new Blob(this.PartialCaptureFrames));
                var img = document.createElement("img");
                img.onload = () => {
                    UI.Screen2DContext.drawImage(img, left, top, width, height);
                    window.URL.revokeObjectURL(url);
                };
                img.src = url;
                this.PartialCaptureFrames = [];
            }
            else {
                this.PartialCaptureFrames.push(buffer);
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
        hubConnection.on("ConnectionRequestDenied", () => {
            this.Connection.stop();
            UI.StatusMessage.innerHTML = "Connection request denied.";
            UI.ShowMessage("Connection request denied.");
        });
        hubConnection.on("Unauthorized", () => {
            UI.ConnectButton.removeAttribute("disabled");
            UI.StatusMessage.innerHTML = "Authorization failed.";
            UI.ShowMessage("Authorization failed.");
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
            MainViewer.ClientID = newClientID;
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
        hubConnection.on("ReceiveRtcOffer", async (sdp, iceServers) => {
            console.log("Rtc offer SDP received.");
            MainViewer.RtcSession.Init(iceServers);
            await MainViewer.RtcSession.ReceiveRtcOffer(sdp);
        });
        hubConnection.on("ReceiveIceCandidate", (candidate, sdpMlineIndex, sdpMid) => {
            console.log("Ice candidate received.");
            MainViewer.RtcSession.ReceiveCandidate({
                candidate: candidate,
                sdpMLineIndex: sdpMlineIndex,
                sdpMid: sdpMid
            });
        });
        hubConnection.on("ShowMessage", (message) => {
            UI.ShowMessage(message);
        });
        hubConnection.on("WindowsSessions", (windowsSessions) => {
            UI.UpdateWindowsSessions(windowsSessions);
        });
    }
}
//# sourceMappingURL=ViewerHubConnection.js.map