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
import { GetSettings } from "./SettingsService.js";


var queryString = Utilities.ParseSearchString();

export const ViewerApp = {
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
    ViewOnlyMode: queryString["viewonly"] ?
        decodeURIComponent(queryString["viewonly"]).toLowerCase() == "true" :
        false,
    Mode: RemoteControlMode.None,
    Settings: GetSettings(),

    Init: () => {
        if (ViewerApp.ViewOnlyMode) {
            UI.ViewOnlyButton.classList.add("toggled");
        }

        ApplyInputHandlers();

        if (queryString["clientID"]) {
            ViewerApp.Mode = RemoteControlMode.Unattended;
            UI.ConnectBox.style.display = "none";
            ViewerApp.ViewerHubConnection.Connect();
        }
        else if (queryString["sessionID"]) {
            UI.SessionIDInput.value = decodeURIComponent(queryString["sessionID"]);
            if (queryString["requesterName"]) {
                UI.RequesterNameInput.value = decodeURIComponent(queryString["requesterName"]);
                ViewerApp.ConnectToClient();
            }
        }
    },
    ConnectToClient: () => {
        UI.ConnectButton.disabled = true;
        ViewerApp.ClientID = UI.SessionIDInput.value.split(" ").join("");
        ViewerApp.RequesterName = UI.RequesterNameInput.value;
        ViewerApp.Mode = RemoteControlMode.Normal;
        ViewerApp.ViewerHubConnection.Connect();
        UI.StatusMessage.innerHTML = "Sending connection request...";
    }
}