import { UserSettings} from "./UserSettings.js";
import * as Utilities from "./Utilities.js";
import { GetSelectedDevices } from "./DataGrid.js";


export var CommandCompletionDiv = document.querySelector("#commandCompletionDiv") as HTMLDivElement;
export var CommandInfoDiv = document.querySelector("#commandInfoDiv") as HTMLDivElement;
export var CommandModeSelect = document.querySelector("#commandModeSelect") as HTMLSelectElement;
export var ConsoleOutputDiv = document.querySelector("#consoleOutputDiv") as HTMLDivElement;
export var ConsoleTextArea = document.querySelector("#consoleTextArea") as HTMLTextAreaElement;
export var DeviceGrid = document.querySelector("#deviceGrid") as HTMLTableElement;
export var DevicesSelectedCount = document.querySelector("#devicesSelectedSpan") as HTMLSpanElement;
export var OnlineDevicesCount = document.querySelector("#onlineDevicesSpan") as HTMLSpanElement;
export var TotalDevicesCount = document.querySelector("#totalDevicesSpan") as HTMLSpanElement;
export var MeasurementCanvas = document.createElement("canvas");
export var MeasurementContext = MeasurementCanvas.getContext("2d");
export var TabContentWrapper = document.getElementById("tabContentWrapper") as HTMLDivElement;
export var ConsoleFrame = document.getElementById("consoleFrame") as HTMLDivElement;
export var ConsoleTab = document.getElementById("consoleTab") as HTMLAnchorElement;
export var ConsoleAlert = document.getElementById("consoleAlert") as HTMLAnchorElement;


export function AddConsoleOutput(strOutputMessage:string) {
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
export function AddConsoleHTML(html: string) {
    var contentWrapper = document.createElement("div");
    contentWrapper.innerHTML = html;
    ConsoleOutputDiv.appendChild(contentWrapper);

    ConsoleFrame.scrollTop = ConsoleFrame.scrollHeight;
}
export function AddTransferHarness(transferID: string, totalDevices:number) {
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
export function PopupMessage(message: string) {
    var messageDiv = document.createElement("div");
    messageDiv.classList.add("float-message");
    messageDiv.innerHTML = message;
    document.body.appendChild(messageDiv);
    window.setTimeout(() => {
        messageDiv.remove();
    }, 5000);
}
export function ShowModal(title: string, modalBodyHtml: string, buttonsHTML: string = "", onDismissCallback: VoidFunction = null) : HTMLDivElement {
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
                ${modalBodyHtml}
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
            (ev.currentTarget as HTMLElement).parentElement.remove();
        }
    });
    $("#" + modalID).modal("show");
    return wrapperDiv;
};

export function ValidateInput(inputElement: HTMLInputElement) {
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