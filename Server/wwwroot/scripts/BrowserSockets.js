import * as UI from "./UI.js";
import * as DataGrid from "./DataGrid.js";
import { CreateCommandHarness, AddCommandResultsHarness, AddPSCoreResultsHarness, UpdateResultsCount } from "./ResultsParser.js";
import { Store } from "./Store.js";
import { Main } from "./Main.js";
export var Connection;
export var ServiceID;
export var Connected;
export function Connect() {
    var signalR = window["signalR"];
    Connection = new signalR.HubConnectionBuilder()
        .withUrl("/BrowserHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();
    applyMessageHandlers(Connection);
    Connection.start().catch(err => {
        console.error(err.toString());
        Connected = false;
        UI.AddConsoleOutput("Your connection was lost.  Refresh the page or enter a command to reconnect.");
    }).then(() => {
        Connected = true;
    });
    this.Connection.closedCallbacks.push((ev) => {
        Connected = false;
        if (!Store.IsDisconnectExpected) {
            UI.ShowModal("Connection Failure", "Your connection was lost. Click Reconnect to start a new session.", `<button type="button" class="btn btn-secondary" onclick="location.reload()">Reconnect</button>`);
            UI.AddConsoleOutput("Connection lost.");
        }
    });
}
;
function applyMessageHandlers(hubConnection) {
    hubConnection.on("UserOptions", (options) => {
        Main.UserSettings.CommandModeShortcuts.Web = options.CommandModeShortcutWeb;
        Main.UserSettings.CommandModeShortcuts.PSCore = options.CommandModeShortcutPSCore;
        Main.UserSettings.CommandModeShortcuts.WinPS = options.CommandModeShortcutWinPS;
        Main.UserSettings.CommandModeShortcuts.Bash = options.CommandModeShortcutBash;
        Main.UserSettings.CommandModeShortcuts.CMD = options.CommandModeShortcutCMD;
        UI.AddConsoleOutput("Console connected.");
        DataGrid.RefreshGrid();
    });
    hubConnection.on("LockedOut", (args) => {
        location.assign("/Identity/Account/Lockout");
    });
    hubConnection.on("DeviceCameOnline", (device) => {
        DataGrid.AddOrUpdateDevice(device);
    });
    hubConnection.on("DeviceWentOffline", (device) => {
        DataGrid.AddOrUpdateDevice(device);
    });
    hubConnection.on("DeviceHeartbeat", (device) => {
        DataGrid.AddOrUpdateDevice(device);
    });
    hubConnection.on("RefreshDeviceList", () => {
        DataGrid.RefreshGrid();
    });
    hubConnection.on("PSCoreResult", (result) => {
        AddPSCoreResultsHarness(result);
        UpdateResultsCount(result.CommandContextID);
    });
    hubConnection.on("CommandResult", (result) => {
        AddCommandResultsHarness(result);
        UpdateResultsCount(result.CommandContextID);
    });
    hubConnection.on("DisplayConsoleMessage", (message) => {
        UI.AddConsoleOutput(message);
    });
    hubConnection.on("DisplayConsoleHTML", (message) => {
        UI.AddConsoleHTML(message);
    });
    hubConnection.on("GetGroupsResult", (devices) => {
        var output = `<div>Permission Groups:</div>
                            <table class="console-device-table table table-responsive">
                            <thead><tr>
                            <th>Device Name</th><th>Permission Groups</th>
                            </tr></thead>`;
        var deviceList = devices.map(x => {
            return `<tr>
                        <td>${x.DeviceName}</td>
                        <td>
                            ${x.DevicePermissionLinks.map(x => x.PermissionGroup.Name + "<br />").join("")}
                        </td>
                        </tr>`;
        });
        output += deviceList.join("");
        output += "</table>";
        UI.AddConsoleOutput(output);
    });
    hubConnection.on("TransferCompleted", (transferID) => {
        var completedWrapper = document.getElementById(transferID + "-completed");
        var count = parseInt(completedWrapper.innerHTML);
        completedWrapper.innerHTML = (count + 1).toString();
    });
    hubConnection.on("PSCoreResultViaAjax", (commandID, deviceID) => {
        var targetURL = `${location.origin}/API/Commands/PSCoreResult/${commandID}/${deviceID}`;
        var xhr = new XMLHttpRequest();
        xhr.open("get", targetURL);
        xhr.onload = function () {
            if (xhr.status == 200) {
                AddPSCoreResultsHarness(JSON.parse(xhr.responseText));
                UpdateResultsCount(commandID);
            }
        };
        xhr.send();
    });
    hubConnection.on("WinPSResultViaAjax", (commandID, deviceID) => {
        var targetURL = `${location.origin}/API/Commands/WinPSResult/${commandID}/${deviceID}`;
        var xhr = new XMLHttpRequest();
        xhr.open("get", targetURL);
        xhr.onload = function () {
            if (xhr.status == 200) {
                AddCommandResultsHarness(JSON.parse(xhr.responseText));
                UpdateResultsCount(commandID);
            }
        };
        xhr.send();
    });
    hubConnection.on("CMDResultViaAjax", (commandID, deviceID) => {
        var targetURL = `${location.origin}/API/Commands/PSCoreResult/${commandID}/${deviceID}`;
        var xhr = new XMLHttpRequest();
        xhr.open("get", targetURL);
        xhr.onload = function () {
            if (xhr.status == 200) {
                AddCommandResultsHarness(JSON.parse(xhr.responseText));
                UpdateResultsCount(commandID);
            }
        };
        xhr.send();
    });
    hubConnection.on("BashResultViaAjax", (commandID, deviceID) => {
        var targetURL = `${location.origin}/API/Commands/PSCoreResult/${commandID}/${deviceID}`;
        var xhr = new XMLHttpRequest();
        xhr.open("get", targetURL);
        xhr.onload = function () {
            if (xhr.status == 200) {
                AddCommandResultsHarness(JSON.parse(xhr.responseText));
                UpdateResultsCount(commandID);
            }
        };
        xhr.send();
    });
    hubConnection.on("CommandContextCreated", (context) => {
        UI.AddConsoleHTML(CreateCommandHarness(context).outerHTML);
    });
    hubConnection.on("ServiceID", (serviceID) => {
        ServiceID = serviceID;
    });
    hubConnection.on("UnattendedSessionReady", (rcConnectionID) => {
        window.open(`/RemoteControl?clientID=${rcConnectionID}&serviceID=${ServiceID}`, "_blank");
    });
}
//# sourceMappingURL=BrowserSockets.js.map