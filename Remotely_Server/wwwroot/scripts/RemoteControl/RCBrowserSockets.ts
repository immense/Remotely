import * as Utilities from "../Utilities.js";
import * as UI from "./UI.js";
import { ConnectButton } from "./UI.js";
import { RemoteControl } from "./RemoteControl.js";
import { GetCursor } from "./CursorMap.js";

var signalR = window["signalR"];
var lastFrameDelay = Date.now();

export class RCBrowserSockets {
    Connection: any;

    Connect() {
        this.Connection = new signalR.HubConnectionBuilder()
            .withUrl("/RCBrowserHub")
            .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
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
    SendFrameSkip(delayTime: number) {
        this.Connection.invoke("SendFrameSkip", delayTime);
    }
    SendSelectScreen(index: number) {
        return this.Connection.invoke("SelectScreen", index);
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
    SendTap(): any {
        this.Connection.invoke("Tap");
    }
    SendMouseWheel(deltaX: number, deltaY: number): any {
        this.Connection.invoke("MouseWheel", deltaX, deltaY);
    }
    SendKeyDown(keyCode: number): any {
        this.Connection.invoke("KeyDown", keyCode);
    }
    SendKeyUp(keyCode: number): any {
        this.Connection.invoke("KeyUp", keyCode);
    }
    SendKeyPress(keyCode: number): any {
        this.Connection.invoke("KeyPress", keyCode);
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
            UI.ScreenViewer.width = width;
            UI.ScreenViewer.height = height;
            UI.Screen2DContext.clearRect(0, 0, width, height);
        });
        hubConnection.on("ScreenCapture", (buffer: Uint8Array, captureTime: Date) => {
            //var frameDelay = Date.now() - new Date(captureTime).getTime();
            //if (frameDelay > 3000 && Date.now() - lastFrameDelay > 3000) {
            //    this.SendFrameSkip(frameDelay * .25);
            //    lastFrameDelay = Date.now();
            //}
            var url = window.URL.createObjectURL(new Blob([buffer]));
            var img = document.createElement("img");
            img.onload = () => {
                UI.Screen2DContext.drawImage(img, 0, 0, UI.ScreenViewer.width, UI.ScreenViewer.height);
                window.URL.revokeObjectURL(url);
            };
            img.src = url;
        });
        hubConnection.on("ConnectionFailed", () => {
            UI.ConnectButton.removeAttribute("disabled");
            UI.StatusMessage.innerHTML = "Connection failed or was denied.";
            UI.ShowMessage("Connection failed.  Please reconnect.");
            this.Connection.stop();
        });
        hubConnection.on("SessionIDNotFound", () => {
            UI.ConnectButton.removeAttribute("disabled");
            UI.StatusMessage.innerHTML = "Session ID not found.";
            this.Connection.stop();
        });
        hubConnection.on("ScreenCasterDisconnected", () => {
            this.Connection.stop();
        });
        hubConnection.on("RelaunchedScreenCasterReady", (newClientID: string) => {
            RemoteControl.ClientID = newClientID;
            this.Connection.stop();
            this.Connect();
        });
      
        hubConnection.on("SwitchingDesktops", () => {
            UI.ShowMessage("Switching desktops...");
        });
        hubConnection.on("Reconnecting", () => {
            UI.ShowMessage("Reconnecting...");
        });

        hubConnection.on("CursorChange", (cursor: string) => {
            var newCursor = GetCursor(cursor);
            UI.ScreenViewer.style.cursor = newCursor;
        });
    }
}