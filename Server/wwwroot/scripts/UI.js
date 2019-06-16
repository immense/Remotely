import { UserSettings } from "./UserSettings.js";
import * as Utilities from "./Utilities.js";
import { GetSelectedDevices } from "./DataGrid.js";
export var CommandCompletionDiv = document.querySelector("#commandCompletionDiv");
export var CommandInfoDiv = document.querySelector("#commandInfoDiv");
export var CommandModeSelect = document.querySelector("#commandModeSelect");
export var ConsoleOutputDiv = document.querySelector("#consoleOutputDiv");
export var ConsoleTextArea = document.querySelector("#consoleTextArea");
export var DeviceGrid = document.querySelector("#deviceGrid");
export var DevicesSelectedCount = document.querySelector("#devicesSelectedSpan");
export var OnlineDevicesCount = document.querySelector("#onlineDevicesSpan");
export var TotalDevicesCount = document.querySelector("#totalDevicesSpan");
export var MeasurementCanvas = document.createElement("canvas");
export var MeasurementContext = MeasurementCanvas.getContext("2d");
export var TabContentWrapper = document.getElementById("tabContentWrapper");
export var ConsoleFrame = document.getElementById("consoleFrame");
export var ConsoleTab = document.getElementById("consoleTab");
export var ConsoleAlert = document.getElementById("consoleAlert");
export function AddConsoleOutput(strOutputMessage) {
    var outputBlock = document.createElement("div");
    outputBlock.classList.add("console-block");
    var prompt = document.createElement("div");
    prompt.classList.add("console-prompt");
    prompt.innerHTML = UserSettings.PromptString;
    var output = document.createElement("div");
    output.classList.add("console-output");
    output.innerHTML = strOutputMessage;
    outputBlock.appendChild(prompt);
    outputBlock.appendChild(output);
    ConsoleOutputDiv.appendChild(outputBlock);
    ConsoleFrame.scrollTop = ConsoleFrame.scrollHeight;
    if (!ConsoleTab.classList.contains("active")) {
        ConsoleAlert.hidden = false;
    }
}
export function AddConsoleHTML(html) {
    var contentWrapper = document.createElement("div");
    contentWrapper.innerHTML = html;
    ConsoleOutputDiv.appendChild(contentWrapper);
    ConsoleFrame.scrollTop = ConsoleFrame.scrollHeight;
}
export function AddTransferHarness(transferID, totalDevices) {
    GetSelectedDevices();
    var transferHarness = document.createElement("div");
    transferHarness.id = transferID;
    transferHarness.classList.add("command-harness");
    transferHarness.innerHTML = `
        <div class="command-harness-title">
            File Transfer Status  |  
            Total Devices: ${totalDevices} |  
            Completed: <span id="${transferID}-completed">0</span>
        </div>`;
    AddConsoleHTML(transferHarness.outerHTML);
}
export function AutoSizeTextArea() {
    ConsoleTextArea.style.height = "1px";
    ConsoleTextArea.style.height = Math.max(12, ConsoleTextArea.scrollHeight) + "px";
}
export function FloatMessage(message) {
    var messageDiv = document.createElement("div");
    messageDiv.classList.add("float-message");
    messageDiv.innerHTML = message;
    document.body.appendChild(messageDiv);
    window.setTimeout(() => {
        messageDiv.remove();
    }, 5000);
}
export function ShowModal(title, message, buttonsHTML = "", onDismissCallback = null) {
    var modalID = Utilities.CreateGUID();
    var modalHTML = `<div id="${modalID}" class="modal fade in" tabindex="-1" role="dialog">
          <div class="modal-dialog" role="document">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">${title}</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                  <span aria-hidden="true">&times;</span>
                </button>
              </div>
              <div class="modal-body">
                ${message}
              </div>
              <div class="modal-footer">
                ${buttonsHTML}
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
              </div>
            </div>
          </div>
        </div>`;
    var wrapperDiv = document.createElement("div");
    wrapperDiv.innerHTML = modalHTML;
    document.body.appendChild(wrapperDiv);
    $("#" + modalID).on("hidden.bs.modal", ev => {
        try {
            if (onDismissCallback) {
                onDismissCallback();
            }
        }
        finally {
            ev.currentTarget.parentElement.remove();
        }
    });
    $("#" + modalID)["modal"]();
}
;
export function ValidateInput(inputElement) {
    if (!inputElement.checkValidity()) {
        $(inputElement)["tooltip"]({
            template: '<div class="tooltip" role="tooltip"><div class="arrow"></div><div class="tooltip-inner text-danger"></div></div>',
            title: inputElement.validationMessage
        });
        $(inputElement)["tooltip"]("show");
        return false;
    }
    else {
        return true;
    }
}
//# sourceMappingURL=UI.js.map