import * as Utilities from "../Utilities.js";
import { RCBrowserSockets } from "./RCBrowserSockets.js";
import { RtcSession } from "./RtcSession.js";
import * as UI from "./UI.js";
import { RemoteControlMode } from "../Enums/RemoteControlMode.js";
import { ClipboardWatcher } from "./ClipboardWatcher.js";
import { RtcMessageHandler } from "./RtcMessageHandler.js";


var queryString = Utilities.ParseSearchString();

export const MainRc = {
    ClipboardWatcher: new ClipboardWatcher(),
    Debug: false,
    RCBrowserSockets: new RCBrowserSockets(),
    RtcMessageHandler: new RtcMessageHandler(),
    RtcSession: new RtcSession(),
    ClientID: queryString["clientID"] ? decodeURIComponent(queryString["clientID"]) : "",
    ServiceID: queryString["serviceID"] ? decodeURIComponent(queryString["serviceID"]) : "",
    RequesterName: queryString["requesterName"] ? decodeURIComponent(queryString["requesterName"]) : "",
    Mode: RemoteControlMode.None,

    Init: () => {
        UI.ApplyInputHandlers(MainRc.RCBrowserSockets);

        if (queryString["clientID"]) {
            MainRc.Mode = RemoteControlMode.Unattended;
            UI.ConnectBox.style.display = "none";
            MainRc.RCBrowserSockets.Connect();
        }
        else if (queryString["sessionID"]) {
            UI.SessionIDInput.value = decodeURIComponent(queryString["sessionID"]);
            if (queryString["requesterName"]) {
                UI.RequesterNameInput.value = decodeURIComponent(queryString["requesterName"]);
                this.ConnectToClient();
            }
        }
    },
    ConnectToClient: () => {
        UI.ConnectButton.disabled = true;
        MainRc.ClientID = UI.SessionIDInput.value.split(" ").join("");
        MainRc.RequesterName = UI.RequesterNameInput.value;
        MainRc.Mode = RemoteControlMode.Normal;
        MainRc.RCBrowserSockets.Connect();
        UI.StatusMessage.innerHTML = "Sending connection request...";
    }
}

window["Remotely"] = MainRc;