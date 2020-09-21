import { CommandResult } from "../Shared/Models/CommandResult.js";
import { PSCoreCommandResult } from "../Shared/Models/PSCoreCommandResult.js";
import * as DataGrid from "./DataGrid.js";
import { GenericCommandResult } from "../Shared/Models/GenericCommandResult.js";
import { ConsoleFrame } from "./UI.js";
import { FormatScriptOutput, FormatScriptOutputArray } from "../Shared/Utilities.js";

export function CreateCommandHarness(result: CommandResult): HTMLDivElement {
    var collapseClass = result.TargetDeviceIDs.length > 1 ? "collapse" : "collapse show";
    var commandHarness = document.createElement("div");
    var contextID = "c" + result.ID;
    commandHarness.id = contextID;
    commandHarness.classList.add("command-harness");
    commandHarness.innerHTML = `
        <div class="command-harness-title">
            Command Type: ${result.CommandMode}  |  
            Total Devices: <span id="${contextID}-totaldevices">${result.TargetDeviceIDs.length}</span>  |  
            Completed: <span id="${contextID}-completed">0%</span>  |
            Errors: <span id="${contextID}-errors">0</span>  |  
            <button class="btn btn-sm btn-secondary" data-toggle="collapse" data-target='#${contextID}-results'>View</button> 
            <a class="btn btn-sm btn-secondary" target="_blank" href="${location.origin}/API/Commands/JSON/${result.ID}">JSON</a>
            <a class="btn btn-sm btn-secondary" target="_blank" href="${location.origin}/API/Commands/XML/${result.ID}">XML</a> 
        </div>
        <div id="${contextID}-results" class="${collapseClass}">
        </div>`;
    return commandHarness;
}

export function AddPSCoreResultsHarness(result: PSCoreCommandResult) {
    var contextID = "c" + result.CommandResultID;
    var deviceName = DataGrid.DataSource.find(x => x.ID == result.DeviceID).DeviceName;
    var resultsWrapper = document.getElementById(contextID + "-results");
    var totalDevices = parseInt(document.getElementById(contextID + "-totaldevices").innerText);
    var collapseClass = totalDevices > 1 ? "collapse" : "collapse show";
    
    var resultDiv = document.createElement("div");
    resultDiv.innerHTML = `
        <div class="result-header">
                Device: ${deviceName}  |  
                Had Errors: ${result.ErrorOutput.length > 1 ? "Yes": "No"}  |  
                <button class="btn btn-sm btn-secondary" data-toggle="collapse" data-target='#${contextID + result.DeviceID}-result'>View</button>
        </div>
        <div id='${contextID + result.DeviceID}-result' class="command-result-output ${collapseClass}">
            <div>Host Output:<br>${FormatScriptOutput(result.HostOutput)}</div>
            <div>Debug Output:<br>${FormatScriptOutputArray(result.DebugOutput)}</div>
            <div>Verbose Output:<br>${FormatScriptOutputArray(result.VerboseOutput)}</div>
            <div>Information Output:<br>${FormatScriptOutputArray(result.InformationOutput)}</div>
            <div>Error Output:<br>${FormatScriptOutputArray(result.ErrorOutput)}</div>
        </div>`;
    if (result.ErrorOutput.length > 0) {
        var errorSpan = document.getElementById(contextID + "-errors");
        var currentErrors = parseInt(errorSpan.innerText);
        currentErrors += 1;
        errorSpan.innerText = String(currentErrors);
    }
    resultsWrapper.appendChild(resultDiv);
    ConsoleFrame.scrollTop = ConsoleFrame.scrollHeight;
}
export function AddCommandResultsHarness(result: GenericCommandResult) {
    var contextID = "c" + result.CommandResultID;
    var deviceName = DataGrid.DataSource.find(x => x.ID == result.DeviceID).DeviceName;
    var resultsWrapper = document.getElementById(contextID + "-results");
    var totalDevices = parseInt(document.getElementById(contextID + "-totaldevices").innerText);
    var collapseClass = totalDevices > 1 ? "collapse" : "collapse show";
    
    var resultDiv = document.createElement("div");
    resultDiv.innerHTML = `
        <div class="result-header">
                Device: ${deviceName}  |  
                Had Errors: ${result.ErrorOutput.length > 1 ? "Yes" : "No"}  |  
                <button class="btn btn-sm btn-secondary" data-toggle="collapse" data-target="#${contextID + result.DeviceID}-result">View</button>
        </div>
        <div id="${contextID + result.DeviceID}-result" class="command-result-output ${collapseClass}">
            <div>Standard Output:<br>${FormatScriptOutput(result.StandardOutput)}</div>
            <div>Error Output:<br>${FormatScriptOutput(result.ErrorOutput)}</div>
        </div>`;
    if (result.ErrorOutput.length > 0) {
        var errorSpan = document.getElementById(`${contextID}-errors`);
        var currentErrors = parseInt(errorSpan.innerText);
        currentErrors += 1;
        errorSpan.innerText = String(currentErrors);
    }
    resultsWrapper.appendChild(resultDiv);
    ConsoleFrame.scrollTop = ConsoleFrame.scrollHeight;
}

export function UpdateResultsCount(commandResultID: string) {
    var contextID = "c" + commandResultID;
    var totalDevices = parseInt(document.getElementById(`${contextID}-totaldevices`).innerText);
    var percentComplete = Math.round(document.getElementById(`${contextID}-results`).children.length / totalDevices * 100);
    document.getElementById(`${contextID}-completed`).innerText = String(percentComplete) + "%";
}