import * as Utilities from "../Utilities.js";
import { RCBrowserSockets } from "./RCBrowserSockets.js";
import { RtcSession } from "./RtcSession.js";
import * as UI from "./UI.js";
import { RemoteControlMode } from "../Enums/RemoteControlMode.js";
var queryString = Utilities.ParseSearchString();
export const Remotely = {
    RCBrowserSockets: new RCBrowserSockets(),
    RtcSession: new RtcSession(),
    ClientID: queryString["clientID"] ? decodeURIComponent(queryString["clientID"]) : "",
    ServiceID: queryString["serviceID"] ? decodeURIComponent(queryString["serviceID"]) : "",
    RequesterName: queryString["requesterName"] ? decodeURIComponent(queryString["requesterName"]) : "",
    Mode: RemoteControlMode.None,
    Init: () => {
        UI.ApplyInputHandlers(Remotely.RCBrowserSockets);
        if (queryString["clientID"]) {
            Remotely.Mode = RemoteControlMode.Unattended;
            UI.ConnectBox.style.display = "none";
            Remotely.RCBrowserSockets.Connect();
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
        Remotely.ClientID = UI.SessionIDInput.value.split(" ").join("");
        Remotely.RequesterName = UI.RequesterNameInput.value;
        Remotely.Mode = RemoteControlMode.Normal;
        Remotely.RCBrowserSockets.Connect();
        UI.StatusMessage.innerHTML = "Sending connection request...";
    }
};
window["Remotely"] = Remotely;
//# sourceMappingURL=Main.js.map