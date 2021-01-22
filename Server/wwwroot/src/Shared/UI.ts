import { CreateGUID } from "./Utilities.js";

export var ToastsWrapper = document.getElementById("toastsWrapper") as HTMLDivElement;

export function AppendChild(parentElement: HTMLElement, childContent: string, childTag: string) {
    var childElement = document.createElement(childTag);
    childElement.innerText = childContent;
    parentElement.appendChild(childElement);
}


export function ShowMessage(message: string) {
    var messageDiv = document.createElement("div");
    messageDiv.classList.add("toast-message");
    messageDiv.innerHTML = message;
    ToastsWrapper.appendChild(messageDiv);
    window.setTimeout(() => {
        messageDiv.remove();
    }, 5000);
}

export function ShowModal(title: string, modalBodyHtml: string, buttonsHTML: string = "", onDismissCallback: VoidFunction = null): HTMLDivElement {
    var modalID = CreateGUID();
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
}

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