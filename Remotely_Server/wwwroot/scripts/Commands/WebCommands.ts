import { ConsoleCommand } from "../Models/ConsoleCommand.js"
import { Parameter } from "../Models/Parameter.js";
import { Machine } from "../Models/Machine.js";
import * as UI from "../UI.js";
import * as BrowserSockets from "../BrowserSockets.js";
import { CommandLineParameter } from "../Models/CommandLineParameter.js";
import { Main } from "../Main.js";
import * as DataGrid from "../DataGrid.js";
import { AddConsoleHTML, AddConsoleOutput } from "../UI.js";
import { GetSelectedMachines } from "../DataGrid.js";


var commands: Array<ConsoleCommand> = [
    new ConsoleCommand(
        "AddGroup",
        [
            new Parameter("group", "The group name to add.", "String")
        ],
        "Adds a permission group to the selected computer(s).",
        "AddGroup -group Lab Machines",
        "",
        (parameters, paramDictionary) => {
            var selectedMachines = Main.DataGrid.GetSelectedMachines();
            if (selectedMachines.length == 0) {
                AddConsoleOutput("No machines are selected.");
                return;
            };
            if (!paramDictionary["group"]) {
                AddConsoleOutput("A group name is required.");
                return;
            }
            BrowserSockets.Connection.invoke("AddGroup", selectedMachines.map(x=>x.ID), paramDictionary["group"]);
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
        "Deploy and run a script on the selected machine(s).  Note: Only one switch argument can be used.",
        "deployscript -pscore",
        "",
        (parameters, parameterDict) => {
            if (parameters.length != 1) {
                AddConsoleOutput("There should be exactly 1 argument to indicate the script type.");
                return;
            }
            var selectedMachines = Main.DataGrid.GetSelectedMachines();
            if (selectedMachines.length == 0) {
                AddConsoleOutput("No machines are selected.");
                return;
            }
            var fileInput = document.createElement("input");
            fileInput.type = "file";
            fileInput.hidden = true;
            document.body.appendChild(fileInput);
            fileInput.onchange = () => {
                uploadFiles(fileInput.files).then(value => {
                    BrowserSockets.Connection.invoke("DeployScript", value[0], parameters[0].Name, selectedMachines.map(x => x.ID));
                    fileInput.remove();
                });
            }
            fileInput.click();
        }
    ),
    new ConsoleCommand(
        "RemoveGroup",
        [
            new Parameter("group", "The group name to remove.", "String")
        ],
        "Removes a permission group to the selected computer(s).",
        "RemoveGroup -group Lab Machines",
        "",
        (parameters, paramDictionary) => {
            var selectedMachines = Main.DataGrid.GetSelectedMachines();
            if (selectedMachines.length == 0) {
                AddConsoleOutput("No machines are selected.");
                return;
            };
            if (!paramDictionary["group"]) {
                AddConsoleOutput("A group name is required.");
                return;
            }
            BrowserSockets.Connection.invoke("RemoveGroup", selectedMachines.map(x => x.ID), paramDictionary["group"]);
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
        (parameters, paramDictionary) => { BrowserSockets.Connect(); }
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
            UI.AddConsoleOutput(paramDictionary["message"]);
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
                UI.AddConsoleOutput("No matching commands found.");
            }
            else if (result.length == 1) {
                UI.AddConsoleHTML("<br>" + result[0].FullHelp);
            }
            else {
                var outputText = "Multiple commands found: <br><br>";
                for (var i = 0; i < result.length; i++) {
                    outputText += result[i].Name + "<br>";
                }
                UI.AddConsoleHTML(outputText);
            }
        }
    ),
    new ConsoleCommand(
        "List",
        [],
        "Displays a list of the currently-selected machines.",
        "list",
        "",
        () => {
            var selectedMachines = Main.DataGrid.GetSelectedMachines();

            if (selectedMachines.length == 0) {
                UI.AddConsoleOutput("No machines selected.");
                return;
            }
            var output = `<div>Selected Machines:</div>
                            <table class="console-machine-table table table-responsive">
                            <thead><tr>
                            <th>Online</th><th>Machine Name</th><th>Current User</th><th>Last Online</th><th>Platform</th><th>OS Description</th><th>Free Storage</th><th>Total Storage (GB)</th><th>Free Memory</th><th>Total Memory (GB)</th><th>Tags</th>
                            </tr></thead>`;
            
            var machineList = selectedMachines.map(x => {
                return `<tr>
                        <td>${String(x.IsOnline)
                                .replace("true", "<span class='fa fa-check-circle'></span>")
                                .replace("false", "<span class='fa fa-times'></span>")}</td>
                        <td>${x.MachineName}</td>
                        <td>${x.CurrentUser}</td>
                        <td>${new Date(x.LastOnline).toLocaleString()}</td>
                        <td>${x.Platform}</td>
                        <td>${x.OSDescription}</td>
                        <td>${Math.round(x.FreeStorage * 100)}%</td>
                        <td>${x.TotalStorage.toLocaleString()}</td>
                        <td>${Math.round(x.FreeMemory * 100)}%</td>
                        <td>${x.TotalMemory.toLocaleString()}</td>
                        <td>${x.Tags}</td>
                        </tr>`
            });
            output += machineList.join("");
            UI.AddConsoleOutput(output);
        }
    ),
    new ConsoleCommand(
        "Remove",
        [

        ],
        "Removes the selected machines from the database.  (Note: This does not attempt to uninstall the client.  Use Uninstall.)",
        "remove",
        "",
        (parameters) => {
            var machines = DataGrid.GetSelectedMachines();
            if (machines.length == 0) {
                UI.AddConsoleOutput("No machines selected.");
            }
            else {
                BrowserSockets.Connection.invoke("RemoveMachines", machines.map(x=>x.ID));
            }
        }
    ),
    new ConsoleCommand(
        "Select",
        [
            new Parameter("all", "If specified, all machines will be selected.", "Switch"),
            new Parameter("none", "If specified, no machines will be selected.", "Switch"),
            new Parameter("online", "If specified, selects the machines that are currently online.", "Switch"),
            new Parameter("filter", "A comma-separated list of search filters (spaces are optional).  See the help article for more details.", "String"),
            new Parameter("append", "If specified, selected machines will be added list of computers already selected (if any).", "Switch"),
        ],
        "Selects machines based on a supplied filter.",
        "select -filter isonline=true, machinename*Rig, freestorage<50",
        `The filter contains a comma-separated list of fields to search, and machines that match all filters will be selected.
            There are multiple operator types, and they can be used on any of the fields on the grid (plus some extra hidden ones).<br><br>
            Operators:<br>
            <ul style="list-style:none">
                <li>=&nbsp;&nbsp;&nbsp;  Exactly equal to. "machinename=myrig" would match "MyRig", but not "MyRig2"</li>
                <li>*&nbsp;&nbsp;&nbsp;  Like. "currentuser*bus" would match "Business" and "A_Busy_User"</li>
                <li>!=&nbsp;&nbsp;       Not equal to. "currentuser!=jon" would match every user except "Jon"</li>
                <li>!*&nbsp;&nbsp;       Not like. "currentuser!*jon" would match "Jon", "Jonathan", and "SuperJon"</li>
                <li>>&nbsp;&nbsp;&nbsp;  Greater than. "totalmemory>10" would match computers with more than 10GB of RAM</li>
                <li><&nbsp;&nbsp;&nbsp;  Less than. "freestorage<0.1" would match computers with less than 10% disk space left</li>
            </ul>

            Fields:<br>
                <ul style="list-style:none">
                    <li>IsOnline (true or false)</li>
                    <li>MachineName (text)</li>
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
                </ul>`,
        (parameters, paramDictionary) => {

            if (typeof paramDictionary["all"] != "undefined") {
                Main.DataGrid.DataSource.forEach(x => {
                    document.getElementById(x.ID).classList.add("row-selected");
                })
                UI.AddConsoleOutput(`${GetSelectedMachines().length} machines selected.`);
            }
            if (typeof paramDictionary["none"] != "undefined") {
                Main.UI.MachineGrid.querySelectorAll(".row-selected").forEach(x => {
                    x.classList.remove("row-selected");
                });
                UI.AddConsoleOutput(`${GetSelectedMachines().length} machines selected.`);
            }
            if (typeof paramDictionary["online"] != "undefined") {
                Main.DataGrid.DataSource.filter(x => x.IsOnline).forEach(x => {
                    document.getElementById(x.ID).classList.add("row-selected");
                });
                UI.AddConsoleOutput(`${GetSelectedMachines().length} machines selected.`);
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

                        if (key == "lastonline") {
                            var parsedDate = Date.parse(value);
                            switch (operator) {
                                case "=":
                                case "*":
                                case "!=":
                                case "!*":
                                    UI.AddConsoleOutput("Only < and > operators can be used with dates.");
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
                                    lambda += `x[Object.keys(x).find(y=>y.toString().toLowerCase().indexOf("${key}") > -1)].toString().toLowerCase() === "${value}".toString().toLowerCase() && `;
                                    break;
                                case "*":
                                    lambda += `x[Object.keys(x).find(y=>y.toString().toLowerCase().indexOf("${key}") > -1)].toString().toLowerCase().search("${value}".toString().toLowerCase()) > -1 && `;
                                    break;
                                case "!=":
                                    lambda += `x[Object.keys(x).find(y=>y.toString().toLowerCase().indexOf("${key}") > -1)].toString().toLowerCase() !== "${value}".toString().toLowerCase() && `;
                                    break;
                                case "!*":
                                    lambda += `x[Object.keys(x).find(y=>y.toString().toLowerCase().indexOf("${key}") > -1)].toString().toLowerCase().search("${value}".toString().toLowerCase()) === -1 && `;
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
                    UI.AddConsoleOutput("Unable to parse filter.  Please check your syntax.");
                    return;
                }
                lambda = lambda.slice(0, lambda.lastIndexOf(" &&"));
                if (paramDictionary["append"] != "true") {
                    Main.UI.MachineGrid.querySelectorAll(".row-selected").forEach(x => {
                        x.classList.remove("row-selected");
                    });
                }
                var selectedMachines = Main.DataGrid.DataSource.filter(x => eval(lambda));
                selectedMachines.forEach(x => {
                    document.getElementById(x.ID).classList.add("row-selected");
                });
                UI.AddConsoleOutput(`${GetSelectedMachines().length} machines selected.`);
            }

            Main.DataGrid.UpdateMachineCounts();
        }
    ),
    new ConsoleCommand(
        "RemoteControl",
        [],
        "Connect to a computer with Remotely Remote Control.",
        "list",
        "",
        () => {
            var selectedMachines = Main.DataGrid.GetSelectedMachines();
            if (selectedMachines.length == 0) {
                UI.AddConsoleOutput("You must select a machine first.");
                return;
            }
            if (selectedMachines.length > 1) {
                UI.AddConsoleOutput("You can only initiate remote control on one machine at a time.");
                return;
            }
            UI.AddConsoleOutput("Launching remote control on client machine...");
            BrowserSockets.Connection.invoke("RemoteControl", selectedMachines[0].ID);
        }
    ),
    new ConsoleCommand("Uninstall",
        [],
        "Uninstalls the Remotely client from the selected machines.  Warning: This can't be undone from the web portal.  You would need to redeploy the client.",
        "uninstall",
        "",
        (parameters, parameterDict) => {
            var selectedMachines = DataGrid.GetSelectedMachines();
            BrowserSockets.Connection.invoke("UninstallClients", selectedMachines.map(x=>x.ID));
        }
    ),
    new ConsoleCommand(
        "SetTags",
        [
            new Parameter("Tags", "The tags to set for the machine. Max length is 200 characters.", "String"),
            new Parameter("Append", "Append the tags to any existing ones.", "Switch")   
        ],
        "Set the tag/description for the selected machines.  Max length is 200 characters.",
        "SetTags -tags John's computer, Portland office, desktop",
        "",
        (parameters, parameterDict) => {
            var selectedMachines = Main.DataGrid.GetSelectedMachines();
            if (selectedMachines.length == 0) {
                AddConsoleOutput("No machines are selected.");
                return;
            }
            selectedMachines.forEach(x => {
                if (typeof parameterDict["append"] != 'undefined') {
                    var separator = x.Tags.trim().endsWith(",") ? " " : ", ";
                    parameterDict["tags"] = x.Tags.trim() + separator + parameterDict["tags"]
                }
                DataGrid.DataSource.find(y => y.ID == x.ID).Tags = parameterDict["tags"];
                (document.getElementById(x.ID).querySelector(".machine-tag") as HTMLInputElement).value = parameterDict["tags"];
                BrowserSockets.Connection.invoke("UpdateTags", x.ID, parameterDict["tags"]);
            });
        }
    ),
    new ConsoleCommand(
        "TransferFiles",
        [
        ],
        "Transfer file(s) to the selected machines.",
        "transferfiles",
        "",
        (parameters, parameterDict) => {
            var selectedMachines = Main.DataGrid.GetSelectedMachines();
            if (selectedMachines.length == 0) {
                AddConsoleOutput("No machines are selected.");
                return;
            }
            var transferID = Main.Utilities.CreateGUID();
            UI.AddTransferHarness(transferID, selectedMachines.length);
            var fileInput = document.createElement("input");
            fileInput.type = "file";
            fileInput.hidden = true;
            fileInput.multiple = true;
            document.body.appendChild(fileInput);
            fileInput.onchange = () => {
                uploadFiles(fileInput.files).then(value => {
                    BrowserSockets.Connection.invoke("TransferFiles", value, transferID, selectedMachines.map(x => x.ID));
                    fileInput.remove();
                });
            }
            fileInput.click();
        }
    )
];

function uploadFiles(fileList: FileList): Promise<string[]> {
    return new Promise<string[]>((resolve, reject) => {
        UI.AddConsoleOutput("File upload started...");

        var strPath = "/API/FileSharing/";
        var fd = new FormData();
        for (var i = 0; i < fileList.length; i++) {
            fd.append('fileUpload' + i, fileList[i]);
        }
        var xhr = new XMLHttpRequest();
        xhr.open('POST', strPath, true);
        xhr.addEventListener("load", function () {
            if (xhr.status === 200) {
                UI.AddConsoleOutput("File upload completed.");
                resolve(JSON.parse(xhr.responseText));
            }
            else {
                UI.AddConsoleOutput("File upload failed.");
                reject();
            }
        });
        xhr.addEventListener("error", () => {
            UI.AddConsoleOutput("File upload failed.");
            reject();
        });
        xhr.addEventListener("progress", function (e) {
            UI.AddConsoleOutput("File upload progress: " + String(isFinite(e.loaded / e.total) ? e.loaded / e.total : 0) + "%");
        });
        xhr.send(fd);
    })
   
}

export const WebCommands = commands;
