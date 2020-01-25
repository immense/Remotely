import * as UI from "./UI.js";
import * as DataGrid from "./DataGrid.js";
import { Device } from "./Models/Device.js";
import { PSCoreCommandResult } from "./Models/PSCoreCommandResult.js";
import { GenericCommandResult } from "./Models/GenericCommandResult.js";
import { CommandContext } from "./Models/CommandContext.js";
import { CreateCommandHarness, AddCommandResultsHarness, AddPSCoreResultsHarness, UpdateResultsCount } from "./ResultsParser.js";
import { Store } from "./Store.js";
import { UserOptions } from "./Models/UserOptions.js";
import { Main } from "./Main.js";
import { AddConsoleOutput, AddConsoleHTML } from "./Console.js";


export var Connection: any;
export var ServiceID: string;
export var Connected: boolean;

export function Connect() {
    var signalR = window["signalR"];
    Connection = new signalR.HubConnectionBuilder()
        .withUrl("/BrowserHub")
        .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
        .configureLogging(signalR.LogLevel.Information)
        .build();

    applyMessageHandlers(Connection);

    Connection.start().catch(err => {
        console.error(err.toString());
        Connected = false;
        AddConsoleOutput("Your connection was lost.  Refresh the page or enter a command to reconnect.");
    }).then(() => {
        Connected = true;
    })
    this.Connection.closedCallbacks.push((ev) => {
        Connected = false;
        if (!Store.IsDisconnectExpected) {
            UI.ShowModal("Connection Failure",
                "Your connection was lost. Click Reconnect to start a new session.",
                `<button type="button" class="btn btn-secondary" onclick="location.reload()">Reconnect</button>`);
            AddConsoleOutput("Connection lost.");
        }
    });
};

function applyMessageHandlers(hubConnection) {
    hubConnection.on("Chat", (deviceName: string, message: string) => {
        AddConsoleHTML(`<strong class="text-info">Chat from ${deviceName}</strong>: ${message}`);
    });
    hubConnection.on("UserOptions", (options: UserOptions) => {
        Main.UserSettings.CommandModeShortcuts.Web = options.CommandModeShortcutWeb;
        Main.UserSettings.CommandModeShortcuts.PSCore = options.CommandModeShortcutPSCore;
        Main.UserSettings.CommandModeShortcuts.WinPS = options.CommandModeShortcutWinPS;
        Main.UserSettings.CommandModeShortcuts.Bash = options.CommandModeShortcutBash;
        Main.UserSettings.CommandModeShortcuts.CMD = options.CommandModeShortcutCMD;
        AddConsoleOutput("Console connected.");
        DataGrid.RefreshGrid();
    });
    hubConnection.on("LockedOut", (args) => {
        location.assign("/Identity/Account/Lockout");
    });

    hubConnection.on("DeviceCameOnline", (device:Device) => {
        DataGrid.AddOrUpdateDevice(device);
    });
    hubConnection.on("DeviceWentOffline", (device: Device) => {
        DataGrid.AddOrUpdateDevice(device);
    });
    hubConnection.on("DeviceHeartbeat", (device: Device) => {
        DataGrid.AddOrUpdateDevice(device);
    });

    hubConnection.on("RefreshDeviceList", () => {
        DataGrid.RefreshGrid();
    });

    hubConnection.on("PSCoreResult", (result: PSCoreCommandResult) => {
        AddPSCoreResultsHarness(result);
        UpdateResultsCount(result.CommandContextID);
    });
    hubConnection.on("CommandResult", (result: GenericCommandResult) => {
        AddCommandResultsHarness(result);
        UpdateResultsCount(result.CommandContextID);
    });
    hubConnection.on("DisplayMessage", (consoleMessage: string, popupMessage: string) => {
        if (consoleMessage) {
            AddConsoleOutput(consoleMessage);
        }
        if (popupMessage) {
            UI.PopupMessage(popupMessage);
        }
    });
    hubConnection.on("DisplayConsoleHTML", (message: string) => {
        AddConsoleHTML(message);
    });

    hubConnection.on("TransferCompleted", (transferID: string) => {
        var completedWrapper = document.getElementById(transferID + "-completed");
        var count = parseInt(completedWrapper.innerHTML);
        completedWrapper.innerHTML = (count + 1).toString();
    })
    hubConnection.on("PSCoreResultViaAjax", (commandID: string, deviceID: string) => {
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
    hubConnection.on("WinPSResultViaAjax", (commandID: string, deviceID: string) => {
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
    hubConnection.on("CMDResultViaAjax", (commandID: string, deviceID: string) => {
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
    hubConnection.on("BashResultViaAjax", (commandID: string, deviceID: string) => {
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
    hubConnection.on("CommandContextCreated", (context: CommandContext) => {
        AddConsoleHTML(CreateCommandHarness(context).outerHTML);
    });
    hubConnection.on("ServiceID", (serviceID: string) => {
        ServiceID = serviceID;
    });
    hubConnection.on("UnattendedSessionReady", (rcConnectionID: string) => {
        window.open(`/RemoteControl?clientID=${rcConnectionID}&serviceID=${ServiceID}`, "_blank");
    });
}