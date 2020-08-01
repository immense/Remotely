import * as Utilities from "../Utilities.js";
import { RCHubConnection } from "./RCHubConnection.js";
import { RtcSession } from "./RtcSession.js";
import * as UI from "./UI.js";
import { RemoteControlMode } from "../Enums/RemoteControlMode.js";
import { ClipboardWatcher } from "./ClipboardWatcher.js";
import { RtcMessageHandler } from "./RtcMessageHandler.js";
import { MessageSender } from "./MessageSender.js";
import { SessionRecorder } from "./SessionRecorder.js";
import { ApplyInputHandlers } from "./InputEventHandlers.js";


var queryString = Utilities.ParseSearchString();

export const MainRc = {
    ClipboardWatcher: new ClipboardWatcher(),
    MessageSender: new MessageSender(),
    RCHubConnection: new RCHubConnection(),
    RtcMessageHandler: new RtcMessageHandler(),
    RtcSession: new RtcSession(),
    SessionRecorder: new SessionRecorder(),
    ClientID: queryString["clientID"] ? decodeURIComponent(queryString["clientID"]) : "",
    Otp: queryString["otp"] ? decodeURIComponent(queryString["otp"]) : "",
    ServiceID: queryString["serviceID"] ? decodeURIComponent(queryString["serviceID"]) : "",
    RequesterName: queryString["requesterName"] ? decodeURIComponent(queryString["requesterName"]) : "",
    Mode: RemoteControlMode.None,

    Init: () => {
        ApplyInputHandlers();

        if (queryString["clientID"]) {
            MainRc.Mode = RemoteControlMode.Unattended;
            UI.ConnectBox.style.display = "none";
            MainRc.RCHubConnection.Connect();
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
        MainRc.RCHubConnection.Connect();
        UI.StatusMessage.innerHTML = "Sending connection request...";
    }
}

window["Remotely"] = MainRc;