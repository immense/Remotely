import * as DataGrid from "./DataGrid.js";
import { Device } from "../Shared/Models/Device.js";
import { PSCoreCommandResult } from "../Shared/Models/PSCoreCommandResult.js";
import { GenericCommandResult } from "../Shared/Models/GenericCommandResult.js";
import { CommandResult } from "../Shared/Models/CommandResult.js";
import { CreateCommandHarness, AddCommandResultsHarness, AddPSCoreResultsHarness, UpdateResultsCount } from "./ResultsParser.js";
import { UserOptions } from "../Shared/Models/UserOptions.js";
import { MainApp } from "./App.js";
import { AddConsoleOutput, AddConsoleHTML, AddConsoleElement } from "./Console.js";
import { ReceiveChatText } from "./Chat.js";
import { ShowMessage, ShowModal } from "../Shared/UI.js";
import { EncodeForHTML } from "../Shared/Utilities.js";
import { HubConnection } from "../Shared/Models/HubConnection.js";
import { RemoteControlTarget } from "../Shared/Models/RemoteControlTarget.js";

export var BrowserHubConnection = new class BrowserHubConnection {
    Connection: HubConnection;
    DeviceIdToControlTargetLookup: Record<string, RemoteControlTarget> = {};
    Connected: boolean;

    
    public Connect() {
        var signalR = window["signalR"];
        this.Connection = new signalR.HubConnectionBuilder()
            .withUrl("/BrowserHub")
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this.applyMessageHandlers(this.Connection);

        this.Connection.start().catch(err => {
            console.error(err.toString());
            this.Connected = false;
            AddConsoleOutput("Your connection was lost.  Refresh the page or enter a command to reconnect.");
        }).then(() => {
            this.Connected = true;
        })
        this.Connection.closedCallbacks.push((ev) => {
            this.Connected = false;
            ShowModal("Connection Failure",
                "Your connection was lost. Click Reconnect to start a new session.",
                `<button type="button" class="btn btn-secondary" onclick="location.reload()">Reconnect</button>`);
            AddConsoleOutput("Connection lost.");
        });
    }

    public StartRemoteControl(deviceId: string, viewOnlyMode: boolean) {
        var target = new RemoteControlTarget();
        target.ViewOnlyMode = viewOnlyMode;
        this.DeviceIdToControlTargetLookup[deviceId] = target;
        this.Connection.invoke("RemoteControl", deviceId);
    }

    private applyMessageHandlers(hubConnection) {
        hubConnection.on("Chat", (deviceID: string, deviceName: string, message: string, disconnected: boolean) => {
            var encodedMessage = EncodeForHTML(message);
            if (disconnected) {
                AddConsoleHTML("span", "text-info font-italic", `${deviceName} disconnected from chat.`);
            }
            else if (message) {
                AddConsoleHTML("span", "text-info font-weight-bold", `Chat from ${deviceName}:`, message);
            }

            ReceiveChatText(deviceID, deviceName, encodedMessage, disconnected);
        });
        hubConnection.on("UserOptions", (options: UserOptions) => {
            MainApp.UserSettings.CommandModeShortcuts.Web = options.CommandModeShortcutWeb;
            MainApp.UserSettings.CommandModeShortcuts.PSCore = options.CommandModeShortcutPSCore;
            MainApp.UserSettings.CommandModeShortcuts.WinPS = options.CommandModeShortcutWinPS;
            MainApp.UserSettings.CommandModeShortcuts.Bash = options.CommandModeShortcutBash;
            MainApp.UserSettings.CommandModeShortcuts.CMD = options.CommandModeShortcutCMD;
            AddConsoleOutput("Console connected.");
            DataGrid.RefreshGrid();
        });
        hubConnection.on("LockedOut", (args) => {
            location.assign("/Identity/Account/Lockout");
        });

        hubConnection.on("DeviceCameOnline", (device: Device) => {
            DataGrid.AddOrUpdateDevice(device, true);
        });
        hubConnection.on("DeviceWentOffline", (device: Device) => {
            DataGrid.AddOrUpdateDevice(device, true);
        });
        hubConnection.on("DeviceHeartbeat", (device: Device) => {
            DataGrid.AddOrUpdateDevice(device, true);
        });

        hubConnection.on("RefreshDeviceList", () => {
            DataGrid.RefreshGrid();
        });

        hubConnection.on("PSCoreResult", (result: PSCoreCommandResult) => {
            AddPSCoreResultsHarness(result);
            UpdateResultsCount(result.CommandResultID);
        });
        hubConnection.on("CommandResult", (result: GenericCommandResult) => {
            AddCommandResultsHarness(result);
            UpdateResultsCount(result.CommandResultID);
        });
        hubConnection.on("DisplayMessage", (consoleMessage: string, popupMessage: string) => {
            if (consoleMessage) {
                AddConsoleOutput(consoleMessage);
            }
            if (popupMessage) {
                ShowMessage(popupMessage);
            }
        });
        hubConnection.on("DownloadFile", (fileID: string) => {
            location.assign(`/API/FileSharing/${fileID}`);
        });
        hubConnection.on("DownloadFileProgress", (progressPercent: number) => {
            AddConsoleOutput(`Remote computer upload progress: ${progressPercent}%`)
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
        hubConnection.on("CommandResultCreated", (result: CommandResult) => {
            AddConsoleElement(CreateCommandHarness(result));
        });
        hubConnection.on("ServiceID", (deviceId: string, serviceConnectionId: string) => {
            this.DeviceIdToControlTargetLookup[deviceId].ServiceConnectionId = serviceConnectionId;
        });
        hubConnection.on("UnattendedSessionReady", (rcConnectionID: string, deviceId: string) => {
            var target = this.DeviceIdToControlTargetLookup[deviceId];
            var serviceId = target.ServiceConnectionId;
            var viewOnly = target.ViewOnlyMode;
            window.open(`/RemoteControl?clientID=${rcConnectionID}&serviceID=${serviceId}&viewonly=${viewOnly}`, "_blank");
        });
    }
}
