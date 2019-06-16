import * as DataGrid from "./DataGrid.js";
import { ConsoleFrame } from "./UI.js";
export function CreateCommandHarness(context) {
    var collapseClass = context.TargetDeviceIDs.length > 1 ? "collapse" : "collapse show";
    var commandHarness = document.createElement("div");
    var contextID = "c" + context.ID;
    commandHarness.id = contextID;
    commandHarness.classList.add("command-harness");
    commandHarness.innerHTML = `
        <div class="command-harness-title">
            Command Type: ${context.CommandMode}  |  
            Total Devices: <span id="${contextID}-totaldevices">${context.TargetDeviceIDs.length}</span>  |  
            Completed: <span id="${contextID}-completed">0%</span>  |
            Errors: <span id="${contextID}-errors">0</span>  |  
            <button class="btn btn-sm btn-secondary" data-toggle="collapse" data-target='#${contextID}-results'>View</button> 
            <a class="btn btn-sm btn-secondary" target="_blank" href="${location.origin}/API/Commands/JSON/${context.ID}">JSON</a>
            <a class="btn btn-sm btn-secondary" target="_blank" href="${location.origin}/API/Commands/XML/${context.ID}">XML</a> 
        </div>
        <div id="${contextID}-results" class="${collapseClass}">
        </div>`;
    return commandHarness;
}
export function AddPSCoreResultsHarness(result) {
    var contextID = "c" + result.CommandContextID;
    var deviceName = DataGrid.DataSource.find(x => x.ID == result.DeviceID).DeviceName;
    var resultsWrapper = document.getElementById(contextID + "-results");
    var totalDevices = parseInt(document.getElementById(contextID + "-totaldevices").innerText);
    var collapseClass = totalDevices > 1 ? "collapse" : "collapse show";
    var resultDiv = document.createElement("div");
    resultDiv.innerHTML = `
        <div class="result-header">
                Device: ${deviceName}  |  
                Had Errors: ${result.ErrorOutput.length > 1 ? "Yes" : "No"}  |  
                <button class="btn btn-sm btn-secondary" data-toggle="collapse" data-target='#${contextID + result.DeviceID}-result'>View</button>
        </div>
        <div id='${contextID + result.DeviceID}-result' class="command-result-output ${collapseClass}">
            <div>Host Output:<br>${result.HostOutput.replace(/\n/g, "<br>").replace(/ /g, "&nbsp;")}</div>
            <div>Debug Output:<br>${result.DebugOutput.join("<br>").replace(/ /g, "&nbsp;")}</div>
            <div>Verbose Output:<br>${result.VerboseOutput.join("<br>").replace(/ /g, "&nbsp;")}</div>
            <div>Information Output:<br>${result.InformationOutput.join("<br>").replace(/ /g, "&nbsp;")}</div>
            <div>Error Output:<br>${result.ErrorOutput.join("<br>").replace(/ /g, "&nbsp;")}</div>
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
export function AddCommandResultsHarness(result) {
    var contextID = "c" + result.CommandContextID;
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
            <div>Standard Output:<br>${result.StandardOutput.replace(/\n/g, "<br>").replace(/ /g, "&nbsp;")}</div>
            <div>Error Output:<br>${result.ErrorOutput.replace(/\n/g, "<br>").replace(/ /g, "&nbsp;")}</div>
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
export function UpdateResultsCount(commandContextID) {
    var contextID = "c" + commandContextID;
    var totalDevices = parseInt(document.getElementById(`${contextID}-totaldevices`).innerText);
    var percentComplete = Math.round(document.getElementById(`${contextID}-results`).children.length / totalDevices * 100);
    document.getElementById(`${contextID}-completed`).innerText = String(percentComplete) + "%";
}
//# sourceMappingURL=ResultsParser.js.map