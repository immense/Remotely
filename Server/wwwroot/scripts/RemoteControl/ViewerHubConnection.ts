import * as UI from "./UI.js";
import { MainViewer } from "./Main.js";
import { CursorInfo } from "../Shared/Models/CursorInfo.js";
import { Sound } from "../Shared/Sound.js";
import { IceServerModel } from "../Shared/Models/IceServerModel.js";
import { RemoteControlMode } from "../Shared/Enums/RemoteControlMode.js";
import {  WindowsSession } from "./RtcDtos.js";
import { ShowMessage } from "../Shared/UI.js";

var signalR = window["signalR"];

type HubConnection = {
    start: () => Promise<any>;
    connectionStarted: boolean;
    closedCallbacks: any[];
    invoke: (...rest) => any;
    stop: () => any;
}

export class ViewerHubConnection {
    Connection: HubConnection;
    PartialCaptureFrames: Uint8Array[] = [];
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
    ChangeWindowsSession(sessionID: number) {
        if (MainViewer.Mode == RemoteControlMode.Unattended) {
            this.Connection.invoke("ChangeWindowsSession", sessionID);
        }
    }
    SendIceCandidate(candidate: RTCIceCandidate) {
        if (candidate) {
            this.Connection.invoke("SendIceCandidateToAgent", candidate.candidate, candidate.sdpMLineIndex, candidate.sdpMid);
        }
        else {
            this.Connection.invoke("SendIceCandidateToAgent", "", 0, "");
        }
    }
    SendRtcAnswer(sessionDescription: RTCSessionDescription) {
        this.Connection.invoke("SendRtcAnswerToAgent", sessionDescription.sdp);
    }


    SendScreenCastRequestToDevice() {
        this.Connection.invoke("SendScreenCastRequestToDevice", MainViewer.ClientID, MainViewer.RequesterName, MainViewer.Mode, MainViewer.Otp);
    }
    async SendFile(buffer: Uint8Array, fileName: string, messageId: string, endOfFile: boolean, startOfFile: boolean) {
        await this.Connection.invoke("SendFile", buffer, fileName, messageId, endOfFile, startOfFile);
    }
    SendFrameReceived() {
        this.Connection.invoke("SendFrameReceived",);
    }
    SendSelectScreen(displayName: string) {
        this.Connection.invoke("SelectScreen", displayName);
    }
    SendMouseMove(percentX: number, percentY: number): any {
        this.Connection.invoke("MouseMove", percentX, percentY);
    }
    SendMouseDown(button: number, percentX: number, percentY: number): any {
        this.Connection.invoke("MouseDown", button, percentX, percentY);
    }
    SendMouseUp(button: number, percentX: number, percentY: number): any {
        this.Connection.invoke("MouseUp", button, percentX, percentY);
    }
    SendTouchDown(): any {
        this.Connection.invoke("TouchDown");
    }
    SendLongPress(): any {
        this.Connection.invoke("LongPress");
    }
    SendTouchMove(moveX: number, moveY: number): any {
        this.Connection.invoke("TouchMove", moveX, moveY);
    }
    SendTouchUp(): any {
        this.Connection.invoke("TouchUp");
    }
    SendTap(percentX: number, percentY: number): any {
        this.Connection.invoke("Tap", percentX, percentY);
    }
    SendMouseWheel(deltaX: number, deltaY: number): any {
        this.Connection.invoke("MouseWheel", deltaX, deltaY);
    }
    SendKeyDown(key: string): any {
        this.Connection.invoke("KeyDown", key);
    }
    SendKeyUp(key: string): any {
        this.Connection.invoke("KeyUp", key);
    }
    SendKeyPress(key: string): any {
        this.Connection.invoke("KeyPress", key);
    }
    SendSetKeyStatesUp() {
        this.Connection.invoke("SendSetKeyStatesUp");
    }
    SendCtrlAltDel() {
        this.Connection.invoke("CtrlAltDel");
    }
    SendSharedFileIDs(fileIDs: string): any {
        this.Connection.invoke("SendSharedFileIDs", JSON.parse(fileIDs));
    }
    SendQualityChange(qualityLevel: number) {
        this.Connection.invoke("SendQualityChange", qualityLevel);
    }
    SendAutoQualityAdjust(isOn: boolean) {
        this.Connection.invoke("SendAutoQualityAdjust", isOn);
    }
    SendToggleAudio(toggleOn: boolean) {
        this.Connection.invoke("SendToggleAudio", toggleOn);
    };
    SendToggleBlockInput(toggleOn: boolean) {
        this.Connection.invoke("SendToggleBlockInput", toggleOn);
    }
    SendToggleWebRtcVideo(toggleOn: boolean) {
        this.Connection.invoke("SendToggleWebRtcVideo", toggleOn);
    }
    SendClipboardTransfer(text: string, typeText: boolean) {
        this.Connection.invoke("SendClipboardTransfer", text, typeText);
    }
    ToggleConnectUI(shown: boolean) {
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
    private ApplyMessageHandlers(hubConnection) {
        hubConnection.on("ClipboardTextChanged", (clipboardText: string) => {
            MainViewer.ClipboardWatcher.SetClipboardText(clipboardText);
            ShowMessage("Clipboard updated.");
        });
        hubConnection.on("ScreenData", (selectedDisplay: string, displayNames: string[]) => {
            UI.UpdateDisplays(selectedDisplay, displayNames);
        });
        hubConnection.on("ScreenSize", (width: number, height: number) => {
            UI.SetScreenSize(width, height);
        });
        hubConnection.on("ScreenCapture", (buffer: Uint8Array,
            left: number,
            top: number,
            width: number,
            height: number,
            imageQuality: number,
            endOfFrame: boolean) => {

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
        hubConnection.on("AudioSample", (buffer: Uint8Array) => {
            Sound.Play(buffer);
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
        hubConnection.on("ReceiveMachineName", (machineName: string) => {
            document.title = `${machineName} - Remotely Session`;
        });
        hubConnection.on("RelaunchedScreenCasterReady", (newClientID: string) => {
            MainViewer.ClientID = newClientID;
            this.Connection.stop();
            this.Connect();
        });
      
        hubConnection.on("Reconnecting", () => {
            ShowMessage("Reconnecting...");
        });

        hubConnection.on("CursorChange", (cursor: CursorInfo) => {
            UI.UpdateCursor(cursor.ImageBytes, cursor.HotSpot.X, cursor.HotSpot.Y, cursor.CssOverride);
        });

        hubConnection.on("RequestingScreenCast", () => {
            ShowMessage("Requesting remote control...");
        });


        hubConnection.on("ReceiveRtcOffer", async (sdp: string, iceServers: IceServerModel[]) => {
            console.log("Rtc offer SDP received.");
            MainViewer.RtcSession.Init(iceServers);
            await MainViewer.RtcSession.ReceiveRtcOffer(sdp);
            
        });
        hubConnection.on("ReceiveIceCandidate", (candidate: string, sdpMlineIndex: number, sdpMid: string) => {
            console.log("Ice candidate received.");
            MainViewer.RtcSession.ReceiveCandidate({
                candidate: candidate,
                sdpMLineIndex: sdpMlineIndex,
                sdpMid: sdpMid
            } as any);
        });
        hubConnection.on("ShowMessage", (message: string) => {
            ShowMessage(message);
        });
        hubConnection.on("WindowsSessions", (windowsSessions: Array<WindowsSession>) => {
            UI.UpdateWindowsSessions(windowsSessions);
        });
    }
}