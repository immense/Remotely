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
            UI.ShowModal("Connection Failure", "Your connection was lost. Refresh the page or enter a command to reconnect.");
            UI.AddConsoleOutput("Connection lost.");
        }
    });
}
;
function applyMessageHandlers(hubConnection) {
    hubConnection.on("UserOptions", (options) => {
        Main.UserSettings.CommandModeShortcuts.Remotely = options.CommandModeShortcutRemotely;
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
    hubConnection.on("MachineCameOnline", (machine) => {
        DataGrid.AddOrUpdateMachine(machine);
    });
    hubConnection.on("MachineWentOffline", (machine) => {
        DataGrid.AddOrUpdateMachine(machine);
    });
    hubConnection.on("MachineHeartbeat", (machine) => {
        DataGrid.AddOrUpdateMachine(machine);
    });
    hubConnection.on("RefreshMachineList", () => {
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
    hubConnection.on("TransferCompleted", (transferID) => {
        var completedWrapper = document.getElementById(transferID + "-completed");
        var count = parseInt(completedWrapper.innerHTML);
        completedWrapper.innerHTML = (count + 1).toString();
    });
    hubConnection.on("PSCoreResultViaAjax", (commandID, machineID) => {
        var targetURL = `${location.origin}/API/Commands/PSCoreResult/${commandID}/${machineID}`;
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
    hubConnection.on("WinPSResultViaAjax", (commandID, machineID) => {
        var targetURL = `${location.origin}/API/Commands/WinPSResult/${commandID}/${machineID}`;
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
    hubConnection.on("CMDResultViaAjax", (commandID, machineID) => {
        var targetURL = `${location.origin}/API/Commands/PSCoreResult/${commandID}/${machineID}`;
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
    hubConnection.on("BashResultViaAjax", (commandID, machineID) => {
        var targetURL = `${location.origin}/API/Commands/PSCoreResult/${commandID}/${machineID}`;
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
    hubConnection.on("UnattendedRTCReady", (rcConnectionID) => {
        window.open(`/RemoteControl?clientID=${rcConnectionID}&serviceID=${ServiceID}`, "_blank");
    });
}
//# sourceMappingURL=BrowserSockets.js.map