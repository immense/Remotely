import * as Utilities from "../Utilities.js";
import * as UI from "./UI.js";
import { RemoteControl } from "./Main.js";
import { CursorInfo } from "../Models/CursorInfo.js";
import { Sound } from "../Sound.js";
import { PopupMessage } from "../UI.js";

var signalR = window["signalR"];

type HubConnection = {
    start: () => Promise<any>;
    closedCallbacks: any[];
    invoke: (...rest) => any;
    stop: () => any;
}

export class RCBrowserSockets {
    Connection: HubConnection;

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
    };

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
        this.Connection.invoke("SendScreenCastRequestToDevice", RemoteControl.ClientID, RemoteControl.RequesterName, RemoteControl.Mode);
    }
    SendLatencyUpdate(sentTime: Date, bytesReceived: number) {
        this.Connection.invoke("SendLatencyUpdate", sentTime, bytesReceived);
    }
    SendSelectScreen(index: number) {
        this.Connection.invoke("SelectScreen", index);
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
    SendClipboardTransfer(text: string, typeText: boolean) {
        this.Connection.invoke("SendClipboardTransfer", text, typeText);
    }
    private ApplyMessageHandlers(hubConnection) {
        hubConnection.on("ClipboardTextChanged", (clipboardText: string) => {
            Utilities.SetClipboardText(clipboardText);
            UI.ClipboardTransferTextArea.value = clipboardText;
            PopupMessage("Clipboard updated.");
        });
        hubConnection.on("ScreenCount", (primaryScreenIndex: number, screenCount: number) => {
            document.querySelector("#screenSelectBar").innerHTML = "";
            for (let i = 0; i < screenCount; i++) {
                var button = document.createElement("button");
                button.innerHTML = `Monitor ${i}`;
                button.classList.add("horizontal-bar-button");
                if (i == primaryScreenIndex) {
                    button.classList.add("toggled");
                }
                document.querySelector("#screenSelectBar").appendChild(button);
                button.onclick = (ev: MouseEvent) => {
                    this.SendSelectScreen(i);
                    document.querySelectorAll("#screenSelectBar .horizontal-bar-button").forEach(button => {
                        button.classList.remove("toggled");
                    });
                    (ev.currentTarget as HTMLButtonElement).classList.add("toggled");
                };
            }
        });
        hubConnection.on("ScreenSize", (width: number, height: number) => {
            UI.ScreenViewer.width = width;
            UI.ScreenViewer.height = height;
            UI.Screen2DContext.clearRect(0, 0, width, height);
        });
        hubConnection.on("ScreenCapture", (buffer: Uint8Array, left:number, top:number, width:number, height:number, captureTime: Date) => {
            //console.log("Websocket frame received.");
            this.SendLatencyUpdate(captureTime, buffer.byteLength);

            var url = window.URL.createObjectURL(new Blob([buffer]));
            var img = document.createElement("img");
            img.onload = () => {
                UI.Screen2DContext.drawImage(img, left, top, width, height);
                window.URL.revokeObjectURL(url);
            };
            img.src = url;
        });
        hubConnection.on("AudioSample", (buffer: Uint8Array) => {
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
        hubConnection.on("ReceiveMachineName", (machineName: string) => {
            document.title = `${machineName} - Remotely Session`;
        });
        hubConnection.on("RelaunchedScreenCasterReady", (newClientID: string) => {
            RemoteControl.ClientID = newClientID;
            this.Connection.stop();
            this.Connect();
        });
      
        hubConnection.on("Reconnecting", () => {
            UI.ShowMessage("Reconnecting...");
        });

        hubConnection.on("CursorChange", (cursor: CursorInfo) => {
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


        hubConnection.on("ReceiveRtcOffer", async (sdp: string) => {
            console.log("Rtc offer SDP received.");
            RemoteControl.RtcSession.Init();
            await RemoteControl.RtcSession.ReceiveRtcOffer(sdp);
            
        });
        hubConnection.on("ReceiveIceCandidate", (candidate: string, sdpMlineIndex: number, sdpMid: string) => {
            console.log("Ice candidate received.");
            RemoteControl.RtcSession.ReceiveCandidate({
                candidate: candidate,
                sdpMLineIndex: sdpMlineIndex,
                sdpMid: sdpMid
            } as any);
        });
        hubConnection.on("ShowMessage", (message: string) => {
            UI.ShowMessage(message);
        });
    }
}