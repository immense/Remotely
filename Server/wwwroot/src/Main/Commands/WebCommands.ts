import { ConsoleCommand } from "../../Shared/Models/ConsoleCommand.js"
import { Parameter } from "../../Shared/Models/Parameter.js";
import * as UI from "../UI.js";
import * as HubConnection from "../HubConnection.js";
import { CommandLineParameter } from "../../Shared/Models/CommandLineParameter.js";
import { MainApp } from "../App.js";
import * as DataGrid from "../DataGrid.js";
import { AddConsoleHTML, AddConsoleOutput, AddTransferHarness } from "../Console.js";
import { GetSelectedDevices } from "../DataGrid.js";
import { EncodeForHTML } from "../../Shared/Utilities.js";


var commands: Array<ConsoleCommand> = [
    new ConsoleCommand(
        "Chat",
        [
            new Parameter("message", "The message to send to the remote device.", "String")
        ],
        "Start a chat session with the selected device.",
        "chat -message Hey, this is your IT guy.",
        "",
        (parameters, paramaterDict) => {
            var selectedDevices = MainApp.DataGrid.GetSelectedDevices();
            if (selectedDevices.length == 0) {
                AddConsoleOutput("You must select a device first.");
                return;
            }

            HubConnection.Connection.invoke("Chat", paramaterDict["message"], selectedDevices.map(x => x.ID));
        }
    ),
    new ConsoleCommand(
        "DeployScript",
        [
            new Parameter("pscore", "Indicates that script should be run with PowerShell Core.", "Switch"),
            new Parameter("winps", "Indicates that script should be run with Windows PowerShell.", "Switch"),
            new Parameter("cmd", "Indicates that script should be run with Windows CMD.", "Switch"),
            new Parameter("bash", "Indicates that script should be run with Bash.", "Switch")
        ],
        "Deploy and run a script on the selected device(s).  Note: Only one switch argument can be used.",
        "deployscript -pscore",
        "",
        (parameters, parameterDict) => {
            if (parameters.length != 1) {
                AddConsoleOutput("There should be exactly 1 argument to indicate the script type.");
                return;
            }
            var selectedDevices = MainApp.DataGrid.GetSelectedDevices();
            if (selectedDevices.length == 0) {
                AddConsoleOutput("No devices are selected.");
                return;
            }
            var fileInput = document.createElement("input");
            fileInput.type = "file";
            fileInput.hidden = true;
            document.body.appendChild(fileInput);
            fileInput.onchange = () => {
                uploadFiles(fileInput.files).then(value => {
                    HubConnection.Connection.invoke("DeployScript", value[0], parameters[0].Name, selectedDevices.map(x => x.ID));
                    fileInput.remove();
                });
            }
            fileInput.click();
        }
    ),
    new ConsoleCommand(
        "DownloadFile",
        [
            new Parameter("path", "The path on the remote computer of the file to download.", "String"),
        ],
        "Download a file from the remote computer.",
        `DownloadFile -path "C:\Users\Me\Pictures\ThatFunnyPic.png"`,
        "",
        (parameters, paramDictionary) => {
            var selectedDevices = MainApp.DataGrid.GetSelectedDevices();
            if (selectedDevices.length == 0) {
                AddConsoleOutput("No devices are selected.");
                return;
            };
            HubConnection.Connection.invoke("DownloadFile", paramDictionary["path"], selectedDevices[0].ID);
        }
    ),
    new ConsoleCommand(
        "GetLogs",
        [
        ],
        "Retrieve the logs from the remote agent.",
        "GetLogs",
        "",
        (parameters, paramDictionary) => {
            var selectedDevices = MainApp.DataGrid.GetSelectedDevices();
            if (selectedDevices.length == 0) {
                AddConsoleOutput("No devices are selected.");
                return;
            };
            HubConnection.Connection.invoke("ExecuteCommandOnClient", "PSCore", 'Get-Content -Path "$([System.IO.Path]::GetTempPath())\\Remotely_Logs.log"', selectedDevices.map(x => x.ID));
        }
    ),
    new ConsoleCommand(
        "GetVersion",
        [
        ],
        "Display the remote agent's version.",
        "GetVersion",
        "",
        (parameters, paramDictionary) => {
            var selectedDevices = MainApp.DataGrid.GetSelectedDevices();
            if (selectedDevices.length == 0) {
                AddConsoleOutput("No devices are selected.");
                return;
            };
            var output = `<div>Version Results:</div>
                            <table class="console-device-table table table-responsive">
                            <thead><tr>
                            <th>Device Name</th><th>Agent Version</th>
                            </tr></thead>`;

            var deviceList = selectedDevices.map(x => {
                return `<tr>
                        <td>${x.DeviceName}</td>
                        <td>
                            ${x.AgentVersion}
                        </td>
                        </tr>`
            });
            output += deviceList.join("");
            output += "</table>";
            AddConsoleOutput(output);
        }
    ),
    new ConsoleCommand(
        "Clear",
        [
        ],
        "Clears the output window of text.",
        "clear",
        "",
        (parameters, paramDictionary) => { UI.ConsoleOutputDiv.innerHTML = ""; }
    ),
    new ConsoleCommand(
        "Connect",
        [
        ],
        "Connect or reconnect to the server.",
        "connect",
        "",
        (parameters, paramDictionary) => { HubConnection.Connect(); }
    ),
    new ConsoleCommand(
        "Echo",
        [
            new Parameter("message", "The text to display in the console.", "String")
        ],
        "Writes a message to the console window.",
        "echo -message This will appear in the console.",
        "",
        (parameters, paramDictionary) => {
            AddConsoleOutput(paramDictionary["message"]);
        }
    ),
    new ConsoleCommand(
        "ExpandResults",
        [
            
        ],
        "Expands the results of the last scripting command.",
        "expandresults",
        "",
        (parameters, paramDictionary) => {
            $(UI.ConsoleOutputDiv).find(".command-harness").last().find(".collapse")['collapse']('show');
        }
    ),
    new ConsoleCommand(
        "CollapseResults",
        [

        ],
        "Collapses all scripting results.",
        "collapseresults",
        "",
        (parameters, paramDictionary) => {
            $(UI.ConsoleOutputDiv).find(".command-harness").find(".collapse")['collapse']('hide');
        }
    ),
    new ConsoleCommand(
        "Help",
        [
            new Parameter("command", "The name of the command to look up.", "String")
        ],
        "Displays the full help text for a command.",
        "help -command Select",
        "",
        (parameters) => {
            if (parameters.length == 0) {
                var output = `Command List:<br><div class="help-list">`;
                WebCommands.forEach(x => {
                    output += `<div>${x.Name}</div><div>${x.Summary}</div>`;
                })
                output += "</div>";
                AddConsoleOutput(output);
                return;
            }
            var suppliedCommand = parameters.find(x => x.Name.toLowerCase() == "command") || {} as CommandLineParameter;
            var result = commands.filter(command => {
                return command.Name.toLowerCase() == (suppliedCommand.Value || "").toLowerCase();
            });
            if (result.length == 0) {
                AddConsoleOutput("No matching commands found.");
            }
            else if (result.length == 1) {
                AddConsoleHTML("<br>" + result[0].FullHelp);
            }
            else {
                var outputText = "Multiple commands found: <br><br>";
                for (var i = 0; i < result.length; i++) {
                    outputText += result[i].Name + "<br>";
                }
                AddConsoleHTML(outputText);
            }
        }
    ),
    new ConsoleCommand(
        "List",
        [],
        "Displays a list of the currently-selected devices.",
        "list",
        "",
        () => {
            var selectedDevices = MainApp.DataGrid.GetSelectedDevices();

            if (selectedDevices.length == 0) {
                AddConsoleOutput("No devices are selected.");
                return;
            }
            var output = `<div>Selected Devices:</div>
                            <table class="console-device-table table table-responsive">
                            <thead><tr>
                            <th>Online</th><th>Device Name</th><th>Alias</th><th>Current User</th><th>Last Online</th><th>Platform</th><th>OS Description</th><th>Free Storage</th><th>Total Storage (GB)</th><th>Free Memory</th><th>Total Memory (GB)</th><th>Tags</th>
                            </tr></thead>`;
            
            var deviceList = selectedDevices.map(x => {
                return `<tr>
                        <td>${String(x.IsOnline)
                                .replace("true", "<span class='fa fa-check-circle'></span>")
                                .replace("false", "<span class='fa fa-times'></span>")}</td>
                        <td>${EncodeForHTML(x.DeviceName)}</td>
                        <td>${EncodeForHTML(x.Alias)}</td>
                        <td>${EncodeForHTML(x.CurrentUser)}</td>
                        <td>${new Date(x.LastOnline).toLocaleString()}</td>
                        <td>${EncodeForHTML(x.Platform)}</td>
                        <td>${EncodeForHTML(x.OSDescription)}</td>
                        <td>${Math.round(x.UsedStorage / x.TotalStorage * 100)}%</td>
                        <td>${EncodeForHTML(x.TotalStorage.toLocaleString())}</td>
                        <td>${Math.round(x.UsedMemory / x.TotalMemory * 100)}%</td>
                        <td>${EncodeForHTML(x.TotalMemory.toLocaleString())}</td>
                        <td>${EncodeForHTML(x.Tags || "")}</td>
                        </tr>`
            });
            output += deviceList.join("");
            output += "</table>";
            AddConsoleOutput(output);
        }
    ),
    new ConsoleCommand(
        "Remove",
        [

        ],
        "Removes the selected devices from the database.  (Note: This does not attempt to uninstall the client.  Use Uninstall.)",
        "remove",
        "",
        (parameters) => {
            var devices = DataGrid.GetSelectedDevices();
            if (devices.length == 0) {
                AddConsoleOutput("No devices are selected.");
            }
            else {
                HubConnection.Connection.invoke("RemoveDevices", devices.map(x=>x.ID));
            }
        }
    ),
    new ConsoleCommand(
        "Select",
        [
            new Parameter("all", "If specified, all devices will be selected.", "Switch"),
            new Parameter("none", "If specified, no devices will be selected.", "Switch"),
            new Parameter("online", "If specified, selects the devices that are currently online.", "Switch"),
            new Parameter("filter", "A comma-separated list of search filters (spaces are optional).  See the help article for more details.", "String"),
            new Parameter("append", "If specified, selected devices will be added list of computers already selected (if any).", "Switch"),
        ],
        "Selects devices based on a supplied filter.",
        "select -filter isonline=true, devicename*Rig, freestorage<50",
        `The filter contains a comma-separated list of fields to search, and devices that match all filters will be selected.
            There are multiple operator types, and they can be used on any of the fields on the grid (plus some extra hidden ones).<br><br>
            Operators:<br>
            <ul style="list-style:none">
                <li>=&nbsp;&nbsp;&nbsp;  Exactly equal to. "devicename=myrig" would match "MyRig", but not "MyRig2"</li>
                <li>*&nbsp;&nbsp;&nbsp;  Like. "currentuser*bus" would match "Business" and "A_Busy_User"</li>
                <li>!=&nbsp;&nbsp;       Not equal to. "currentuser!=jon" would match every user except "Jon"</li>
                <li>!*&nbsp;&nbsp;       Not like. "currentuser!*jon" would match "Jon", "Jonathan", and "SuperJon"</li>
                <li>>&nbsp;&nbsp;&nbsp;  Greater than. "totalmemory>10" would match computers with more than 10GB of RAM</li>
                <li><&nbsp;&nbsp;&nbsp;  Less than. "freestorage<0.1" would match computers with less than 10% disk space left</li>
            </ul>

            Fields:<br>
                <ul style="list-style:none">
                    <li>IsOnline (true or false)</li>
                    <li>DeviceName (text)</li>
                    <li>Alias (text)</li>
                    <li>CurrentUser (text)</li>
                    <li>LastOnline (date or date+time</li>
                    <li>Is64Bit (true or false)</li>
                    <li>OSDescription (text)</li>
                    <li>Platform (text)</li>
                    <li>FreeStorage (between 0 and 1)</li>
                    <li>TotalStorage (number in GB)</li>
                    <li>FreeMemory (between 0 and 1)</li>
                    <li>TotalMemory (number in GB)</li>
                    <li>ProcessorCount (number)</li>
                    <li>PublicIP (string)</li>
                    <li>Tags (text)</li>
                    <li>Notes (text)</li>
                </ul>`,
        (parameters, paramDictionary) => {

            if (typeof paramDictionary["all"] != "undefined") {
                MainApp.DataGrid.DataSource.forEach(x => {
                    document.getElementById(x.ID).classList.add("row-selected");
                })
                AddConsoleOutput(`${GetSelectedDevices().length} devices selected.`);
            }
            if (typeof paramDictionary["none"] != "undefined") {
                MainApp.UI.DeviceGrid.querySelectorAll(".row-selected").forEach(x => {
                    x.classList.remove("row-selected");
                });
                AddConsoleOutput(`${GetSelectedDevices().length} devices selected.`);
            }
            if (typeof paramDictionary["online"] != "undefined") {
                MainApp.DataGrid.DataSource.filter(x => x.IsOnline).forEach(x => {
                    document.getElementById(x.ID).classList.add("row-selected");
                });
                AddConsoleOutput(`${GetSelectedDevices().length} devices selected.`);
            }
            if (typeof paramDictionary["filter"] != "undefined") {
                try {
                    var lambda = "";
                    var filterString = paramDictionary["filter"];
                    var filters = filterString.split(",") as string[];
                    filters.forEach(filter => {
                        var operatorIndex = filter.search(/([^\w|\d|\s])/i);
                        var valueIndex = filter.slice(operatorIndex).search(/([\w|\d|\s|.])/i) + operatorIndex;
                        var key = filter.slice(0, operatorIndex).trim().toLowerCase();
                        var operator = filter.slice(operatorIndex, valueIndex).trim().toLowerCase();
                        var value = filter.slice(valueIndex).trim().toLowerCase();
                        console.log(`Searching by key ${key}, operator ${operator}, value ${value}`);
                        if (key == "lastonline") {
                            var parsedDate = Date.parse(value);
                            switch (operator) {
                                case "=":
                                case "*":
                                case "!=":
                                case "!*":
                                    AddConsoleOutput("Only < and > operators can be used with dates.");
                                    return;
                                case ">":
                                    lambda += `Date.parse(x[Object.keys(x).find(y=>y.toLowerCase().indexOf("${key}") > -1)]) > ${parsedDate} && `;
                                    break;
                                case "<":
                                    lambda += `Date.parse(x[Object.keys(x).find(y=>y.toLowerCase().indexOf("${key}") > -1)]) < ${parsedDate} && `;
                                    break;
                                default:
                                    throw "Unable to parse comparison operator.";
                            }
                        }
                        else {
                            switch (operator) {
                                case "=":
                                    lambda += `(String(x[Object.keys(x).find(y=>y.toString().toLowerCase().indexOf("${key}") > -1)])).toLowerCase() === "${value}".toString().toLowerCase() && `;
                                    break;
                                case "*":
                                    lambda += `(String(x[Object.keys(x).find(y=>y.toString().toLowerCase().indexOf("${key}") > -1)])).toLowerCase().search("${value}".toString().toLowerCase()) > -1 && `;
                                    break;
                                case "!=":
                                    lambda += `(String(x[Object.keys(x).find(y=>y.toString().toLowerCase().indexOf("${key}") > -1)])).toLowerCase() !== "${value}".toString().toLowerCase() && `;
                                    break;
                                case "!*":
                                    lambda += `(String(x[Object.keys(x).find(y=>y.toString().toLowerCase().indexOf("${key}") > -1)])).toLowerCase().search("${value}".toString().toLowerCase()) === -1 && `;
                                    break;
                                case ">":
                                    lambda += `parseFloat(x[Object.keys(x).find(y=>y.toString().toLowerCase().indexOf("${key}") > -1)]) > parseFloat("${value}") && `;
                                    break;
                                case "<":
                                    lambda += `parseFloat(x[Object.keys(x).find(y=>y.toString().toLowerCase().indexOf("${key}") > -1)]) < parseFloat("${value}") && `;
                                    break;
                                default:
                                    throw "Unable to parse comparison operator.";
                            }
                        }

                    })
                }
                catch (ex) {
                    AddConsoleOutput("Unable to parse filter.  Please check your syntax.");
                    console.error(ex);
                    return;
                }
                lambda = lambda.slice(0, lambda.lastIndexOf(" &&"));
                if (paramDictionary["append"] != "true") {
                    MainApp.UI.DeviceGrid.querySelectorAll(".row-selected").forEach(x => {
                        x.classList.remove("row-selected");
                    });
                }
                var selectedDevices = MainApp.DataGrid.DataSource.filter(x => {
                    try {
                        return eval(lambda);
                    }
                    catch {
                        return false;
                    }
                });
                selectedDevices.forEach(x => {
                    document.getElementById(x.ID).classList.add("row-selected");
                });
                AddConsoleOutput(`${GetSelectedDevices().length} devices selected.`);
            }

            MainApp.DataGrid.UpdateDeviceCounts();
        }
    ),
    new ConsoleCommand(
        "RemoteControl",
        [],
        "Connect to a computer with Remotely Remote Control.",
        "remotecontrol",
        "",
        () => {
            var selectedDevices = MainApp.DataGrid.GetSelectedDevices();
            if (selectedDevices.length == 0) {
                AddConsoleOutput("You must select a device first.");
                return;
            }
            if (selectedDevices.length > 1) {
                AddConsoleOutput("You can only initiate remote control on one device at a time.");
                return;
            }
            AddConsoleOutput("Launching remote control on client device...");
            HubConnection.Connection.invoke("RemoteControl", selectedDevices[0].ID);
        }
    ),
    new ConsoleCommand("Uninstall",
        [],
        "Uninstalls the Remotely client from the selected devices.  Warning: This can't be undone from the web portal.  You would need to redeploy the client.",
        "uninstall",
        "",
        (parameters, parameterDict) => {
            var selectedDevices = DataGrid.GetSelectedDevices();
            HubConnection.Connection.invoke("UninstallAgents", selectedDevices.map(x=>x.ID));
        }
    ),
    new ConsoleCommand(
        "SetTags",
        [
            new Parameter("Tags", "The tags to set for the device. Max length is 200 characters.", "String"),
            new Parameter("Append", "Append the tags to any existing ones.", "Switch")   
        ],
        "Set the tag/description for the selected devices.  Max length is 200 characters.",
        "SetTags -tags John's computer, Portland office, desktop",
        "",
        (parameters, parameterDict) => {
            var selectedDevices = MainApp.DataGrid.GetSelectedDevices();
            if (selectedDevices.length == 0) {
                AddConsoleOutput("No devices are selected.");
                return;
            }
            selectedDevices.forEach(x => {
                if (typeof parameterDict["append"] != 'undefined') {
                    var separator = x.Tags.trim().endsWith(",") ? " " : ", ";
                    parameterDict["tags"] = x.Tags.trim() + separator + parameterDict["tags"]
                }
                DataGrid.DataSource.find(y => y.ID == x.ID).Tags = parameterDict["tags"];
                HubConnection.Connection.invoke("UpdateTags", x.ID, parameterDict["tags"]);
            });
        }
    ),
    new ConsoleCommand(
        "UploadFiles",
        [
        ],
        "Upload file(s) to the selected devices.  The files get saved in C:\\Windows\\Temp on the remote computer.",
        "uploadfiles",
        "",
        (parameters, parameterDict) => {
            var selectedDevices = MainApp.DataGrid.GetSelectedDevices();
            if (selectedDevices.length == 0) {
                AddConsoleOutput("No devices are selected.");
                return;
            }
            var transferID = MainApp.Utilities.CreateGUID();
            AddTransferHarness(transferID, selectedDevices.length);
            var fileInput = document.createElement("input");
            fileInput.type = "file";
            fileInput.hidden = true;
            fileInput.multiple = true;
            document.body.appendChild(fileInput);
            fileInput.onchange = () => {
                uploadFiles(fileInput.files).then(value => {
                    HubConnection.Connection.invoke("UploadFiles", value, transferID, selectedDevices.map(x => x.ID));
                    fileInput.remove();
                });
            }
            fileInput.click();
        }
    )
];

function uploadFiles(fileList: FileList): Promise<string[]> {
    return new Promise<string[]>((resolve, reject) => {
        AddConsoleOutput("File upload started.");

        var strPath = "/API/FileSharing/";
        var fd = new FormData();
        for (var i = 0; i < fileList.length; i++) {
            fd.append('fileUpload' + i, fileList[i]);
        }
        var xhr = new XMLHttpRequest();
        xhr.open('POST', strPath, true);
        xhr.addEventListener("load", function () {
            if (xhr.status === 200) {
                AddConsoleOutput("File upload completed.  It might take a while for the agent to download it.");
                resolve(JSON.parse(xhr.responseText));
            }
            else {
                AddConsoleOutput("File upload failed.");
                reject();
            }
        });
        xhr.addEventListener("error", () => {
            AddConsoleOutput("File upload failed.");
            reject();
        });

        xhr.upload.onprogress = (e) => {
            var currentPercent = isFinite(e.loaded / e.total) ? Math.round(e.loaded / e.total * 100) : 0;

            if (currentPercent > uploadPercent) {
                var uploadPercent = currentPercent;
                AddConsoleOutput("File upload progress: " + String(currentPercent) + "%");
            }
        };

        xhr.send(fd);
    })
   
}

export const WebCommands = commands;
