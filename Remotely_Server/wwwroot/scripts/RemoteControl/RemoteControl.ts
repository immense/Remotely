import * as Utilities from "../Utilities.js";
import { RCBrowserSockets } from "./RCBrowserSockets.js";
import { BrowserRTC } from "./BrowserRTC.js";
import * as UI from "./UI.js";


var queryString = Utilities.ParseSearchString();
var rcBrowserSockets = new RCBrowserSockets();
var browserRTC = new BrowserRTC();

export const RemoteControl = new class {
    RCBrowserSockets = rcBrowserSockets;
    BrowserRTC = browserRTC;
    ClientID = queryString["clientID"] ? decodeURIComponent(queryString["clientID"]) : undefined;
    ServiceID = queryString["serviceID"] ? decodeURIComponent(queryString["serviceID"]) : undefined;
    Mode: "Unattended" | "Normal";
    RequesterName: string;
}

export function ConnectToClient() {
    UI.ConnectButton.disabled = true;
    RemoteControl.ClientID = UI.SessionIDInput.value.split(" ").join("");
    RemoteControl.RequesterName = UI.RequesterNameInput.value;
    RemoteControl.Mode = "Normal";
    RemoteControl.RCBrowserSockets.Connect();
    UI.StatusMessage.innerHTML = "Sending connection request...";
}

window.onload = () => {
    UI.ApplyInputHandlers(rcBrowserSockets, browserRTC);

    if (queryString["clientID"]) {
        RemoteControl.Mode = "Unattended";
        UI.ConnectBox.style.display = "none";
        RemoteControl.RCBrowserSockets.Connect();
    }
    else if (queryString["sessionID"]) {
        UI.SessionIDInput.value = decodeURIComponent(queryString["sessionID"]);
        if (queryString["requesterName"]) {
            UI.RequesterNameInput.value = decodeURIComponent(queryString["requesterName"]);
            ConnectToClient();
        }
    }
}