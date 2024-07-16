import * as Utilities from "./Utilities.js";
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
    SessionRecorder: new SessionRecorder(),
    SessionId: queryString["sessionId"] ? decodeURIComponent(queryString["sessionId"]) : "",
    AccessKey: queryString["accessKey"] ? decodeURIComponent(queryString["accessKey"]) : "",
    RequesterName: queryString["requesterName"] ? decodeURIComponent(queryString["requesterName"]) : "",
    ViewOnlyMode: queryString["viewonly"] ?
        decodeURIComponent(queryString["viewonly"]).toLowerCase() == "true" :
        false,
    Mode: RemoteControlMode.Unknown,
    Settings: GetSettings(),
    Init: () => {
        ViewerApp.Mode = queryString["mode"] ?
            RemoteControlMode[decodeURIComponent(queryString["mode"])] :
            RemoteControlMode.Attended;
        if (ViewerApp.ViewOnlyMode) {
            UI.ViewOnlyButton.classList.add("toggled");
        }
        ApplyInputHandlers();
        if (UI.RequesterNameInput.value) {
            ViewerApp.RequesterName = UI.RequesterNameInput.value;
        }
        else if (ViewerApp.Settings.DisplayName) {
            UI.RequesterNameInput.value = ViewerApp.Settings.DisplayName;
            ViewerApp.RequesterName = ViewerApp.Settings.DisplayName;
        }
        if (ViewerApp.Mode == RemoteControlMode.Unattended) {
            // Ctrl+Alt+Del only works when screen caster is launched from
            // a service (i.e. unattended mode).
            UI.CtrlAltDelButton.classList.remove("d-none");
            UI.WindowsSessionMenuButton.classList.remove("d-none");
            ViewerApp.ViewerHubConnection.Connect();
            UI.StatusMessage.innerHTML = "Connecting to remote device";
        }
        else {
            UI.SessionIDInput.value = ViewerApp.SessionId;
            UI.RequesterNameInput.value = ViewerApp.RequesterName;
            UI.ToggleConnectUI(true);
        }
    },
    ConnectToClient: () => {
        ViewerApp.SessionId = UI.SessionIDInput.value.split(" ").join("").trim();
        if (!ViewerApp.SessionId) {
            UI.ShowToast("Session ID is required");
            UI.SetStatusMessage("Session ID is required.");
            return;
        }
        UI.ConnectButton.disabled = true;
        UI.ConnectButton.innerText = "Requesting remote control";
        ViewerApp.RequesterName = UI.RequesterNameInput.value;
        ViewerApp.Mode = RemoteControlMode.Attended;
        ViewerApp.ViewerHubConnection.Connect();
        ViewerApp.Settings.DisplayName = ViewerApp.RequesterName;
        SetSettings(ViewerApp.Settings);
    }
};
window["ViewerApp"] = ViewerApp;
//# sourceMappingURL=App.js.map