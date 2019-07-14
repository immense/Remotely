import * as Utilities from "../Utilities.js";
import { RCBrowserSockets } from "./RCBrowserSockets.js";
import * as UI from "./UI.js";
import { RemoteControlMode } from "../Enums/RemoteControlMode.js";
import { Conductor } from "./Conductor.js";


var queryString = Utilities.ParseSearchString();
var rcBrowserSockets = new RCBrowserSockets();

export const RemoteControl = new Conductor(rcBrowserSockets,
    queryString["clientID"] ? decodeURIComponent(queryString["clientID"]) : "",
    queryString["serviceID"] ? decodeURIComponent(queryString["serviceID"]) : "",
    queryString["requesterName"] ? decodeURIComponent(queryString["requesterName"]) : "");

export function ConnectToClient() {
    UI.ConnectButton.disabled = true;
    RemoteControl.ClientID = UI.SessionIDInput.value.split(" ").join("");
    RemoteControl.RequesterName = UI.RequesterNameInput.value;
    RemoteControl.Mode = RemoteControlMode.Normal;
    RemoteControl.RCBrowserSockets.Connect();
    UI.StatusMessage.innerHTML = "Sending connection request...";
}

window.onload = () => {
    UI.ApplyInputHandlers(rcBrowserSockets);

    if (queryString["clientID"]) {
        RemoteControl.Mode = RemoteControlMode.Unattended;
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