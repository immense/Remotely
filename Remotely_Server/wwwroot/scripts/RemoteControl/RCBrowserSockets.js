import * as UI from "./UI.js";
import { RemoteControl } from "./RemoteControl.js";
var signalR = window["signalR"];
export class RCBrowserSockets {
    Connect() {
        this.Connection = new signalR.HubConnectionBuilder()
            .withUrl("/RCBrowserHub")
            .configureLogging(signalR.LogLevel.Information)
            .build();
        this.ApplyMessageHandlers(this.Connection);
        this.Connection.start().catch(err => {
            console.error(err.toString());
            console.log("Connection closed.");
        }).then(() => {
            this.Connection.invoke("GetIceConfiguration");
        });
        this.Connection.closedCallbacks.push((ev) => {
            console.log("Connection closed.");
            UI.StatusMessage.innerHTML = "Connection closed.";
            UI.ScreenViewer.setAttribute("hidden", "hidden");
            UI.ConnectBox.style.removeProperty("display");
        });
    }
    ;
    SendOfferRequestToDevice() {
        return this.Connection.invoke("SendOfferRequestToDevice", RemoteControl.ClientID, RemoteControl.RequesterName, RemoteControl.Mode);
    }
    SendIceCandidate(candidate) {
        return this.Connection.invoke("SendIceCandidateToDevice", candidate, RemoteControl.Mode, RemoteControl.ClientID);
    }
    SendRTCSession(description) {
        return this.Connection.invoke("SendRTCSessionToDevice", description, RemoteControl.Mode, RemoteControl.ClientID);
    }
    SendSelectScreen(index) {
        return this.Connection.invoke("SelectScreen", index);
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
    SendTap() {
        this.Connection.invoke("Tap");
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
        this.Connection.invoke("CtrlAltDel", RemoteControl.ServiceID);
    }
    SendSharedFileIDs(fileIDs) {
        this.Connection.invoke("SendSharedFileIDs", JSON.parse(fileIDs));
    }
    ApplyMessageHandlers(hubConnection) {
        hubConnection.on("IceConfiguration", (iceConfiguration) => {
            RemoteControl.BrowserRTC.IceConfiguration = iceConfiguration;
            RemoteControl.BrowserRTC.Init();
            this.SendOfferRequestToDevice();
        });
        hubConnection.on("RTCSession", (description) => {
            UI.ConnectButton.removeAttribute("disabled");
            UI.ConnectBox.style.display = "none";
            UI.ScreenViewer.removeAttribute("hidden");
            UI.StatusMessage.innerHTML = "";
            RemoteControl.BrowserRTC.ReceiveRTCSession(description);
        });
        hubConnection.on("IceCandidate", (candidate) => {
            RemoteControl.BrowserRTC.ReceiveCandidate(candidate);
        });
        hubConnection.on("ScreenCount", (primaryScreenIndex, screenCount) => {
            document.querySelector("#screenSelectBar").innerHTML = "";
            for (let i = 0; i < screenCount; i++) {
                var button = document.createElement("button");
                button.innerHTML = `Monitor ${i}`;
                button.classList.add("bar-button");
                if (i == primaryScreenIndex) {
                    button.classList.add("toggled");
                }
                document.querySelector("#screenSelectBar").appendChild(button);
                button.onclick = (ev) => {
                    this.SendSelectScreen(i);
                    document.querySelectorAll("#screenSelectBar .bar-button").forEach(button => {
                        button.classList.remove("toggled");
                    });
                    ev.currentTarget.classList.add("toggled");
                };
            }
        });
        hubConnection.on("ConnectionFailed", () => {
            UI.ConnectButton.removeAttribute("disabled");
            UI.StatusMessage.innerHTML = "Connection failed or was denied.";
        });
        hubConnection.on("SessionIDNotFound", () => {
            UI.ConnectButton.removeAttribute("disabled");
            UI.StatusMessage.innerHTML = "Session ID not found.";
        });
        hubConnection.on("RCProcessStopped", () => {
            this.Connection.stop();
        });
        hubConnection.on("DesktopSwitching", () => {
            UI.ShowMessage("Desktop switching in progress...");
        });
        hubConnection.on("SwitchedDesktop", (newClientID) => {
            UI.ShowMessage("Desktop switch completed.");
            RemoteControl.ClientID = newClientID;
            RemoteControl.BrowserRTC.PeerConnection.close();
            RemoteControl.BrowserRTC.Init();
            this.SendOfferRequestToDevice();
        });
        hubConnection.on("DesktopSwitchFailed", () => {
            UI.ShowMessage("Desktop switch failed.  Please reconnect.");
            RemoteControl.BrowserRTC.PeerConnection.close();
        });
    }
}
//# sourceMappingURL=RCBrowserSockets.js.map