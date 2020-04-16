import * as Utilities from "./Utilities.js";


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
export var DeviceGroupSelect = document.getElementById("deviceGroupSelect") as HTMLSelectElement;
export var GridFilter = document.getElementById("gridFilter") as HTMLInputElement;
export var AlertsButton = document.getElementById("alertsButton") as HTMLButtonElement;
export var CloseAlertsButton = document.getElementById("closeAlertsFrameButton") as HTMLButtonElement;
export var AlertsFrame = document.getElementById("alertsFrame") as HTMLDivElement;
export var AlertsCount = document.getElementById("alertsCount") as HTMLSpanElement;
export var ToastsWrapper = document.getElementById("toastsWrapper") as HTMLDivElement;


export function ShowMessage(message: string) {
    var messageDiv = document.createElement("div");
    messageDiv.classList.add("toast-message");
    messageDiv.innerHTML = message;
    ToastsWrapper.appendChild(messageDiv);
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