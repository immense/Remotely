import * as Utilities from "../Utilities.js";
import { BrowserRTC } from "./BrowserRTC.js";
import * as UI from "./UI.js";
import { ConnectButton } from "./UI.js";
import { RemoteControl } from "./RemoteControl.js";

var signalR = window["signalR"];

export class RCBrowserSockets {
    Connection: any;

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
            this.SendScreenCastRequestToDevice();
            UI.ConnectButton.removeAttribute("disabled");
            UI.ConnectBox.style.display = "none";
            UI.ScreenViewer.removeAttribute("hidden");
            UI.StatusMessage.innerHTML = "";
        })
        this.Connection.closedCallbacks.push((ev) => {
            console.log("Connection closed.");
            UI.StatusMessage.innerHTML = "Connection closed.";
            UI.ScreenViewer.setAttribute("hidden", "hidden");
            UI.ConnectBox.style.removeProperty("display");
        });
    };
    SendScreenCastRequestToDevice() {
        return this.Connection.invoke("SendScreenCastRequestToDevice", RemoteControl.ClientID, RemoteControl.RequesterName, RemoteControl.Mode);
    }
    SendIceCandidate(candidate: RTCIceCandidate) {
        return this.Connection.invoke("SendIceCandidateToDevice", candidate, RemoteControl.Mode, RemoteControl.ClientID);
    }
    SendRTCSession(description: RTCSessionDescription) {
        return this.Connection.invoke("SendRTCSessionToDevice", description, RemoteControl.Mode, RemoteControl.ClientID);
    }
    SendSelectScreen(index: number) {
        return this.Connection.invoke("SelectScreen", index);
    }
    SendMouseMove(percentX: number, percentY: number): any {
        this.Connection.invoke("MouseMove", percentX, percentY);
    }
    SendMouseDown(button: string, percentX: number, percentY: number): any {
        this.Connection.invoke("MouseDown", button, percentX, percentY);
    }
    SendMouseUp(button: string, percentX: number, percentY: number): any {
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
    SendTap(): any {
        this.Connection.invoke("Tap");
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
        this.Connection.invoke("CtrlAltDel", RemoteControl.ServiceID);
    }
    SendSharedFileIDs(fileIDs: string): any {
        this.Connection.invoke("SendSharedFileIDs", JSON.parse(fileIDs));
    }
    private ApplyMessageHandlers(hubConnection) {
        hubConnection.on("ScreenCount", (primaryScreenIndex: number, screenCount: number) => {
            document.querySelector("#screenSelectBar").innerHTML = "";
            for (let i = 0; i < screenCount; i++) {
                var button = document.createElement("button");
                button.innerHTML = `Monitor ${i}`;
                button.classList.add("bar-button");
                if (i == primaryScreenIndex) {
                    button.classList.add("toggled");
                }
                document.querySelector("#screenSelectBar").appendChild(button);
                button.onclick = (ev: MouseEvent) => {
                    this.SendSelectScreen(i);
                    document.querySelectorAll("#screenSelectBar .bar-button").forEach(button => {
                        button.classList.remove("toggled");
                    });
                    (ev.currentTarget as HTMLButtonElement).classList.add("toggled");
                };
            }
        });
        hubConnection.on("ScreenSize", (width: number, height: number) => {
            console.log("Screen size received. Width " + width + ".  Height " + height);
            UI.ScreenViewer.width = width;
            UI.ScreenViewer.height = height;
        });
        hubConnection.on("ScreenCapture", (buffer) => {
            UI.Screen2DContext.putImageData(new ImageData(, UI.ScreenViewer.width, UI.ScreenViewer.height), 0, 0);
            //var url = window.URL.createObjectURL(new Blob([buffer]));
            //var img = document.createElement("img");
            //img.onload = function () {
            //    UI.Screen2DContext.drawImage(img, 0, 0, UI.ScreenViewer.width, UI.ScreenViewer.height);
            //    window.URL.revokeObjectURL(url);
            //};
            //img.src = url;
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
        hubConnection.on("SwitchedDesktop", (newClientID: string) => {
            UI.ShowMessage("Desktop switch completed.");
            RemoteControl.ClientID = newClientID;
            RemoteControl.BrowserRTC.PeerConnection.close();
            RemoteControl.BrowserRTC.Init();
            this.SendScreenCastRequestToDevice();
        });
        hubConnection.on("DesktopSwitchFailed", () => {
            UI.ShowMessage("Desktop switch failed.  Please reconnect.");
            RemoteControl.BrowserRTC.PeerConnection.close();
        });
    }
}