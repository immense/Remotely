import * as DataGrid from "./DataGrid.js";
import { TabContentWrapper } from "./UI.js";
export function CreateCommandHarness(context) {
    var collapseClass = context.TargetDeviceIDs.length > 1 ? "collapse" : "collapse show";
    var commandHarness = document.createElement("div");
    commandHarness.id = context.ID;
    commandHarness.classList.add("command-harness");
    commandHarness.innerHTML = `
        <div class="command-harness-title">
            Command Type: ${context.CommandMode}  |  
            Total Devices: <span id="${context.ID}-totaldevices">${context.TargetDeviceIDs.length}</span>  |  
            Completed: <span id="${context.ID}-completed">0%</span>  |
            Errors: <span id="${context.ID}-errors">0</span>  |  
            <button class="btn btn-sm btn-secondary" data-toggle="collapse" data-target='#${context.ID}-results'>View</button> 
            <a class="btn btn-sm btn-secondary" target="_blank" href="${location.origin}/API/Commands/JSON/${context.ID}">JSON</a>
            <a class="btn btn-sm btn-secondary" target="_blank" href="${location.origin}/API/Commands/XML/${context.ID}">XML</a> 
        </div>
        <div id="${context.ID}-results" class="${collapseClass}">
        </div>`;
    return commandHarness;
}
export function AddPSCoreResultsHarness(result) {
    var deviceName = DataGrid.DataSource.find(x => x.ID == result.DeviceID).DeviceName;
    var resultsWrapper = document.getElementById(result.CommandContextID + "-results");
    var totalDevices = parseInt(document.getElementById(result.CommandContextID + "-totaldevices").innerText);
    var collapseClass = totalDevices > 1 ? "collapse" : "collapse show";
    var resultDiv = document.createElement("div");
    resultDiv.innerHTML = `
        <div class="result-header">
                Device: ${deviceName}  |  
                Had Errors: ${result.ErrorOutput.length > 1 ? "Yes" : "No"}  |  
                <button class="btn btn-sm btn-secondary" data-toggle="collapse" data-target='#${result.CommandContextID + result.DeviceID}-result'>View</button>
        </div>
        <div id='${result.CommandContextID + result.DeviceID}-result' class="command-result-output ${collapseClass}">
            <div>Host Output:<br>${result.HostOutput.replace(/\n/g, "<br>").replace(/ /g, "&nbsp;")}</div>
            <div>Debug Output:<br>${result.DebugOutput.join("<br>").replace(/ /g, "&nbsp;")}</div>
            <div>Verbose Output:<br>${result.VerboseOutput.join("<br>").replace(/ /g, "&nbsp;")}</div>
            <div>Information Output:<br>${result.InformationOutput.join("<br>").replace(/ /g, "&nbsp;")}</div>
            <div>Error Output:<br>${result.ErrorOutput.join("<br>").replace(/ /g, "&nbsp;")}</div>
        </div>`;
    if (result.ErrorOutput.length > 0) {
        var errorSpan = document.getElementById(result.CommandContextID + "-errors");
        var currentErrors = parseInt(errorSpan.innerText);
        currentErrors += 1;
        errorSpan.innerText = String(currentErrors);
    }
    resultsWrapper.appendChild(resultDiv);
    TabContentWrapper.scrollTop = TabContentWrapper.scrollHeight;
}
export function AddCommandResultsHarness(result) {
    var deviceName = DataGrid.DataSource.find(x => x.ID == result.DeviceID).DeviceName;
    var resultsWrapper = document.getElementById(result.CommandContextID + "-results");
    var totalDevices = parseInt(document.getElementById(result.CommandContextID + "-totaldevices").innerText);
    var collapseClass = totalDevices > 1 ? "collapse" : "collapse in";
    var resultDiv = document.createElement("div");
    resultDiv.innerHTML = `
        <div class="result-header">
                Device: ${deviceName}  |  
                Had Errors: ${result.ErrorOutput.length > 1 ? "Yes" : "No"}  |  
                <button class="btn btn-sm btn-secondary" data-toggle="collapse" data-target="#${result.CommandContextID + result.DeviceID}-result">View</button>
        </div>
        <div id="${result.CommandContextID + result.DeviceID}-result" class="command-result-output ${collapseClass}">
            <div>Standard Output:<br>${result.StandardOutput.replace(/\n/g, "<br>").replace(/ /g, "&nbsp;")}</div>
            <div>Error Output:<br>${result.ErrorOutput.replace(/\n/g, "<br>").replace(/ /g, "&nbsp;")}</div>
        </div>`;
    if (result.ErrorOutput.length > 0) {
        var errorSpan = document.getElementById(result.CommandContextID + "-errors");
        var currentErrors = parseInt(errorSpan.innerText);
        currentErrors += 1;
        errorSpan.innerText = String(currentErrors);
    }
    resultsWrapper.appendChild(resultDiv);
    TabContentWrapper.scrollTop = TabContentWrapper.scrollHeight;
}
export function UpdateResultsCount(commandContextID) {
    var totalDevices = parseInt(document.getElementById(commandContextID + "-totaldevices").innerText);
    var percentComplete = Math.round(document.getElementById(commandContextID + "-results").children.length / totalDevices * 100);
    document.getElementById(commandContextID + "-completed").innerText = String(percentComplete) + "%";
}
//# sourceMappingURL=ResultsParser.js.map