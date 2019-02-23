import { WebCommands } from "./Commands/WebCommands.js";
import { UserSettings } from "./UserSettings.js";
import { CMDCommands } from "./Commands/CMDCommands.js";
import { PSCoreCommands } from "./Commands/PSCoreCommands.js";
import { BashCommands } from "./Commands/BashCommands.js";
import { Main } from "./Main.js";
import { CommandLineParameter } from "./Models/CommandLineParameter.js";
import * as UI from "./UI.js";
import { Store } from "./Store.js";
import { ConsoleCommand } from "./Models/ConsoleCommand.js";
import { DisplayCommandShortcuts, DisplayCommandCompletions, DisplayParameterCompletions } from "./CommandCompletion.js";
import { WinPSCommands } from "./Commands/WinPSCommands.js";
import { Connection } from "./BrowserSockets.js";

export function EvaluateCurrentCommandText() {
    UI.AutoSizeTextArea();
    
    UI.CommandCompletionDiv.classList.add("hidden");
    UI.CommandInfoDiv.classList.add("hidden");
    UI.CommandCompletionDiv.innerHTML = "";
    Store.CommandCompletionPosition = 0;

    if (UI.ConsoleTextArea.value.startsWith("/")) {
        DisplayCommandShortcuts(UI.ConsoleTextArea.value.slice(1));
        return;
    }
    var relevantText = GetRelevantCommandText(UI.ConsoleTextArea.value);
    var commandInputArray = Main.Utilities.Split(relevantText, " ", 2);
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
            case "Remotely":
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
export function GetCommandCompletions(commandText:string): ConsoleCommand[] {
    var commandList;
    switch (UI.CommandModeSelect.value) {
        case "Web":
            commandList = WebCommands;
            break;
        case "CMD":
            commandList = CMDCommands;
            break;
        case "PSCore":
            commandList = PSCoreCommands;
            break;
        case "WinPS":
            commandList = WinPSCommands;
            break;
        case "Bash":
            commandList = BashCommands;
            break;
        default:
            UI.CommandCompletionDiv.classList.add("hidden");
            return;
    }

    return commandList.filter(x => x.Name.toLowerCase().indexOf(commandText.toLowerCase()) > -1);
}

/** Checks the given string for a matching shortcut. */
export function GetCommandModeShortcut() {
    switch (UI.ConsoleTextArea.value.toLowerCase()) {
        case UserSettings.CommandModeShortcuts.Remotely:
            return "Remotely";
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
    var commandText = UI.ConsoleTextArea.value;
    Store.InputHistoryItems.push(commandText);
    Store.InputHistoryPosition = Store.InputHistoryItems.length;
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
                UI.AddConsoleOutput("Unknown command.");
            }
            break;
        case "PSCore":
        case "WinPS":
        case "CMD":
        case "Bash":
            var allMachines = Main.DataGrid.GetSelectedMachines();
            var windowsMachines = allMachines.filter(x => x.Platform.toLowerCase() == "windows");
            var linuxMachines = allMachines.filter(x => x.Platform.toLowerCase() == "linux");

            if (commandMode == "CMD" && linuxMachines.length > 0) {
                UI.AddConsoleOutput("Linux machines will be excluded from CMD command.");
                allMachines = windowsMachines;
            }
            if (commandMode == "Bash" && windowsMachines.length > 0) {
                UI.AddConsoleOutput("Windows machines will be excluded from Bash command.");
                allMachines = linuxMachines;
            }

            if (allMachines.length == 0) {
                UI.AddConsoleOutput("At least one machine must be selected to send commands.");
                return;
            }
            var machineIDs = allMachines.map(value => value.ID);
            Connection.invoke("ExecuteCommandOnClient", commandMode, commandText, machineIDs );
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
    commandText.substr(startParams).trim().split("-").forEach(x => {
        if (x.trim().length == 0) {
            return;
        }
        var key = "";
        var value = "";
        if (x.indexOf(" ") == -1 || x.substr(x.indexOf(" ")).trim().length == 0) {
            key = x.trim();
        }
        else {
            key = x.substr(0, x.indexOf(" "));
            value = x.substr(x.indexOf(" ")).trim();
        }
        parameterArray.push(new CommandLineParameter(key, value));
       
    });
    return parameterArray;
}