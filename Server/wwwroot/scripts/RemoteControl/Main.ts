import * as Utilities from "../Shared/Utilities.js";
import { RtcSession } from "./RtcSession.js";
import * as UI from "./UI.js";
import { RemoteControlMode } from "../Shared/Enums/RemoteControlMode.js";
import { ClipboardWatcher } from "./ClipboardWatcher.js";
import { DtoMessageHandler } from "./DtoMessageHandler.js";
import { MessageSender } from "./MessageSender.js";
import { SessionRecorder } from "./SessionRecorder.js";
import { ApplyInputHandlers } from "./InputEventHandlers.js";
import { ViewerHubConnection } from "./ViewerHubConnection.js";


var queryString = Utilities.ParseSearchString();

export const MainViewer = {
    ClipboardWatcher: new ClipboardWatcher(),
    MessageSender: new MessageSender(),
    ViewerHubConnection: new ViewerHubConnection(),
    DtoMessageHandler: new DtoMessageHandler(),
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
            MainViewer.Mode = RemoteControlMode.Unattended;
            UI.ConnectBox.style.display = "none";
            MainViewer.ViewerHubConnection.Connect();
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
        MainViewer.ClientID = UI.SessionIDInput.value.split(" ").join("");
        MainViewer.RequesterName = UI.RequesterNameInput.value;
        MainViewer.Mode = RemoteControlMode.Normal;
        MainViewer.ViewerHubConnection.Connect();
        UI.StatusMessage.innerHTML = "Sending connection request...";
    }
}

window["Remotely"] = MainViewer;