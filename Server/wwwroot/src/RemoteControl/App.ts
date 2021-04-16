import * as Utilities from "./Utilities.js";
import { RtcSession } from "./RtcSession.js";
import * as UI from "./UI.js";
import { RemoteControlMode } from "./Enums/RemoteControlMode.js";
import { ClipboardWatcher } from "./ClipboardWatcher.js";
import { DtoMessageHandler } from "./DtoMessageHandler.js";
import { MessageSender } from "./MessageSender.js";
import { SessionRecorder } from "./SessionRecorder.js";
import { ApplyInputHandlers } from "./InputEventHandlers.js";
import { ViewerHubConnection } from "./ViewerHubConnection.js";
import { GetSettings, SetSettings } from "./SettingsService.js";


var queryString = Utilities.ParseSearchString();

export const ViewerApp = {
    ClipboardWatcher: new ClipboardWatcher(),
    MessageSender: new MessageSender(),
    ViewerHubConnection: new ViewerHubConnection(),
    DtoMessageHandler: new DtoMessageHandler(),
    RtcSession: new RtcSession(),
    SessionRecorder: new SessionRecorder(),
    CasterID: queryString["casterID"] ? decodeURIComponent(queryString["casterID"]) : "",
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

        if (UI.RequesterNameInput.value) {
            ViewerApp.RequesterName = UI.RequesterNameInput.value;
        }
        else if (ViewerApp.Settings.displayName) {
            UI.RequesterNameInput.value = ViewerApp.Settings.displayName;
            ViewerApp.RequesterName = ViewerApp.Settings.displayName;
        }

        if (ViewerApp.CasterID) {
            ViewerApp.Mode = RemoteControlMode.Unattended;
            ViewerApp.ViewerHubConnection.Connect();
        }
        else {
            UI.ConnectBox.style.removeProperty("display");
        }

        if (queryString["sessionID"]) {
            UI.SessionIDInput.value = decodeURIComponent(queryString["sessionID"]);
            if (queryString["requesterName"]) {
                UI.RequesterNameInput.value = decodeURIComponent(queryString["requesterName"]);
                ViewerApp.ConnectToClient();
            }
        }
    },
    ConnectToClient: () => {
        UI.ConnectButton.disabled = true;
        ViewerApp.CasterID = UI.SessionIDInput.value.split(" ").join("");
        ViewerApp.RequesterName = UI.RequesterNameInput.value;
        ViewerApp.Mode = RemoteControlMode.Normal;
        ViewerApp.ViewerHubConnection.Connect();
        UI.StatusMessage.innerHTML = "Sending connection request...";

        ViewerApp.Settings.displayName = ViewerApp.RequesterName;
        SetSettings(ViewerApp.Settings);
    }
}

window["ViewerApp"] = ViewerApp;