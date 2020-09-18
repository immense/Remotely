import * as UI from "./UI.js";
import { MainViewer } from "./Main.js";
import { RemoteControlMode } from "../Shared/Enums/RemoteControlMode.js";
import { GenericDto } from "./Dtos.js";
import { ShowMessage } from "../Shared/UI.js";
import { BaseDtoType } from "../Shared/Enums/BaseDtoType.js";
var signalR = window["signalR"];
export class ViewerHubConnection {
    constructor() {
        this.MessagePack = window['MessagePack'];
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
    ChangeWindowsSession(sessionID) {
        if (MainViewer.Mode == RemoteControlMode.Unattended) {
            this.Connection.invoke("ChangeWindowsSession", sessionID);
        }
    }
    SendDtoToClient(dto) {
        return this.Connection.invoke("SendDtoToClient", this.MessagePack.encode(dto));
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
        hubConnection.on("SendDtoToBrowser", (dto) => {
            MainViewer.DtoMessageHandler.ParseBinaryMessage(dto);
        });
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
                this.SendDtoToClient(new GenericDto(BaseDtoType.FrameReceived));
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
        hubConnection.on("ConnectionFailed", () => {
            UI.ConnectButton.removeAttribute("disabled");
            UI.StatusMessage.innerHTML = "Connection failed or was denied.";
            ShowMessage("Connection failed.  Please reconnect.");
            this.Connection.stop();
        });
        hubConnection.on("ConnectionRequestDenied", () => {
            this.Connection.stop();
            UI.StatusMessage.innerHTML = "Connection request denied.";
            ShowMessage("Connection request denied.");
        });
        hubConnection.on("Unauthorized", () => {
            UI.ConnectButton.removeAttribute("disabled");
            UI.StatusMessage.innerHTML = "Authorization failed.";
            ShowMessage("Authorization failed.");
            this.Connection.stop();
        });
        hubConnection.on("ViewerRemoved", () => {
            UI.ConnectButton.removeAttribute("disabled");
            UI.StatusMessage.innerHTML = "The session was stopped by your partner.";
            ShowMessage("Session ended.");
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
            ShowMessage("Reconnecting...");
        });
        hubConnection.on("CursorChange", (cursor) => {
            UI.UpdateCursor(cursor.ImageBytes, cursor.HotSpot.X, cursor.HotSpot.Y, cursor.CssOverride);
        });
        hubConnection.on("RequestingScreenCast", () => {
            ShowMessage("Requesting remote control...");
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
            ShowMessage(message);
        });
        hubConnection.on("WindowsSessions", (windowsSessions) => {
            UI.UpdateWindowsSessions(windowsSessions);
        });
    }
}
//# sourceMappingURL=ViewerHubConnection.js.map