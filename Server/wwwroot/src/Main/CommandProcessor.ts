import { WebCommands } from "./Commands/WebCommands.js";
import { UserSettings } from "./UserSettings.js";
import { MainApp } from "./App.js";
import { CommandLineParameter } from "../Shared/Models/CommandLineParameter.js";
import * as UI from "./UI.js";
import {
    DisplayCommandShortcuts,
    DisplayCommandCompletions,
    DisplayParameterCompletions,
    GetCommandCompletions,
    CommandCompletionStore
} from "./CommandCompletion.js";
import { BrowserHubConnection } from "./BrowserHubConnection.js";
import { AutoSizeTextArea, AddConsoleOutput } from "./Console.js";

export function EvaluateCurrentCommandText() {
    AutoSizeTextArea();
    window.clearTimeout(CommandCompletionStore.CompletionTimeout);
    
    UI.CommandCompletionDiv.classList.add("hidden");
    UI.CommandInfoDiv.classList.add("hidden");
    UI.CommandCompletionDiv.innerHTML = "";
    CommandCompletionStore.CompletionPosition = 0;

    if (UI.ConsoleTextArea.value.startsWith("/")) {
        DisplayCommandShortcuts(UI.ConsoleTextArea.value.slice(1));
        return;
    }
    if (UI.ConsoleTextArea.value.endsWith(" ")) {
        return;
    }
    var relevantText = GetRelevantCommandText(UI.ConsoleTextArea.value);
    var commandInputArray = MainApp.Utilities.Split(relevantText, " ", 2);
    var matchingCommands = GetCommandCompletions(commandInputArray[0]);

    if (commandInputArray.length == 1) {
        if (matchingCommands.length == 0) {
            return;
        }
        DisplayCommandCompletions(matchingCommands, relevantText);    
    }
    else if (commandInputArray.length > 1) {
        switch (UI.CommandModeSelect.value) {
            case "PSCore":
            case "WinPS":
            case "Web":
                var parameters = ExtractParameters(UI.ConsoleTextArea.value);
                DisplayParameterCompletions(matchingCommands[0], parameters, relevantText);
                break;
            default:
                break;;
        }
    }
}
export function GetRelevantCommandText(commandText:string) {
    switch (UI.CommandModeSelect.value) {
        case "PSCore":
        case "WinPS":
        case "Bash":
            var lastLineBreak = Math.max(commandText.lastIndexOf(";"), commandText.lastIndexOf("|"));
            commandText = commandText.substring(lastLineBreak + 1).trim();
            break;
        case "CMD":
            commandText = commandText.substring(commandText.lastIndexOf("&") + 1).trim();
            break;
        default:
            break;;
    }
    return commandText;
}


/** Checks the given string for a matching shortcut. */
export function GetCommandModeShortcut() {
    switch (UI.ConsoleTextArea.value.toLowerCase()) {
        case UserSettings.CommandModeShortcuts.Web:
            return "Web";
        case UserSettings.CommandModeShortcuts.CMD:
            return "CMD";
        case UserSettings.CommandModeShortcuts.PSCore:
            return "PSCore";
        case UserSettings.CommandModeShortcuts.WinPS:
            return "WinPS";
        case UserSettings.CommandModeShortcuts.Bash:
            return "Bash";
        default:
            return null;
    }
}


export function GetCommandMode() {
    return UI.CommandModeSelect.value;
}


/** Processes the command input. */
export function ProcessCommand() {
    var commandText = UI.ConsoleTextArea.value.trim();
    CommandCompletionStore.InputHistoryItems.push(commandText);
    CommandCompletionStore.InputHistoryPosition = CommandCompletionStore.InputHistoryItems.length;
    UI.ConsoleTextArea.value = "";
    var commandMode = UI.CommandModeSelect.value;
    switch (commandMode) {
        case "Web":
            var matchingCommand = WebCommands.find(x => x.Name.toLowerCase() == commandText.split(" ")[0].toLowerCase());
            if (matchingCommand) {
                var parameters = ExtractParameters(commandText);
                // Infer default parameter.
                if (parameters.length > 0 && commandText.indexOf("-") == -1 && matchingCommand.Parameters.length == 1) {
                    parameters = [
                        new CommandLineParameter(matchingCommand.Parameters[0].Name, commandText.substring(commandText.indexOf(" ")).trim())
                    ];
                }
                matchingCommand.Execute(parameters);
            }
            else {
                AddConsoleOutput("Unknown command.");
            }
            break;
        case "PSCore":
        case "WinPS":
        case "CMD":
        case "Bash":
            var allDevices = MainApp.DataGrid.GetSelectedDevices();
            var windowsDevices = allDevices.filter(x => x.Platform.toLowerCase() == "windows");
            var linuxDevices = allDevices.filter(x => x.Platform.toLowerCase() == "linux");

            if (commandMode == "CMD" && linuxDevices.length > 0) {
                AddConsoleOutput("Linux devices will be excluded from CMD command.");
                allDevices = windowsDevices;
            }
            if (commandMode == "Bash" && windowsDevices.length > 0) {
                AddConsoleOutput("Windows devices will be excluded from Bash command.");
                allDevices = linuxDevices;
            }

            if (allDevices.length == 0) {
                AddConsoleOutput("At least one device must be selected to send commands.");
                return;
            }
            var deviceIDs = allDevices.map(value => value.ID);
            BrowserHubConnection.Connection.invoke("ExecuteCommandOnClient", commandMode, commandText, deviceIDs );
            break;
        default:
            break;
    }
}


export function ExtractParameters(commandText: string): Array<CommandLineParameter> {
    var parameterArray = new Array<CommandLineParameter>();
    var startParams = commandText.indexOf(" ");
    if (startParams == -1) {
        return parameterArray;
    }
    commandText.substr(startParams).trim().split(" -").forEach(x => {
        if (x.trim().length == 0) {
            return;
        }
        var kv = x.trim();
        var key = "";
        var value = "";
        if (kv.indexOf(" ") == -1 || kv.substr(kv.indexOf(" ")).trim().length == 0) {
            key = kv;
        }
        else {
            key = kv.substr(0, kv.indexOf(" "));
            value = kv.substr(kv.indexOf(" ")).trim();
        }
        key = key.replace("-", "");
        parameterArray.push(new CommandLineParameter(key, value));
       
    });
    return parameterArray;
}