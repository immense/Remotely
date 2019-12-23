import * as Utilities from "../Utilities.js";
import { RCBrowserSockets } from "./RCBrowserSockets.js";
import * as UI from "./UI.js";
import { RemoteControlMode } from "../Enums/RemoteControlMode.js";
var queryString = Utilities.ParseSearchString();
export const RemoteControl = {
    RCBrowserSockets: new RCBrowserSockets(),
    ClientID: queryString["clientID"] ? decodeURIComponent(queryString["clientID"]) : "",
    ServiceID: queryString["serviceID"] ? decodeURIComponent(queryString["serviceID"]) : "",
    RequesterName: queryString["requesterName"] ? decodeURIComponent(queryString["requesterName"]) : "",
    Mode: RemoteControlMode.None,
    Init: () => {
        UI.ApplyInputHandlers(RemoteControl.RCBrowserSockets);
        if (queryString["clientID"]) {
            RemoteControl.Mode = RemoteControlMode.Unattended;
            UI.ConnectBox.style.display = "none";
            RemoteControl.RCBrowserSockets.Connect();
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
        RemoteControl.ClientID = UI.SessionIDInput.value.split(" ").join("");
        RemoteControl.RequesterName = UI.RequesterNameInput.value;
        RemoteControl.Mode = RemoteControlMode.Normal;
        RemoteControl.RCBrowserSockets.Connect();
        UI.StatusMessage.innerHTML = "Sending connection request...";
    }
};
window["RemoteControl"] = RemoteControl;
//# sourceMappingURL=Main.js.map