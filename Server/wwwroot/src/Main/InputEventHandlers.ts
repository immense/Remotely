import { PositionCommandCompletionWindow, HighlightCompletionWindowItem, CommandCompletionStore } from "./CommandCompletion.js";
import * as UI from "./UI.js";
import * as CommandProcessor from "./CommandProcessor.js";
import * as DataGrid from "./DataGrid.js";
import { BrowserHubConnection } from "./BrowserHubConnection.js";
import { AddConsoleHTML, AddConsoleOutput } from "./Console.js";
import { ShowModal, ShowMessage } from "../Shared/UI.js";


export function ApplyInputEventHandlers() {
    keyDownOnWindow();
    keyDownOnInputTextArea();
    inputOnCommandTextArea();
    inputOnFilterTextBox();
    clickToggleAllDevices();
    clickStartRemoteControlButton();
    consoleTabSelected();
    deviceGroupSelectChanged();
    addAlertHandlers();
    hideOfflineDevicesCheckboxChanged();
    addGridPaginationHandlers();

    window.addEventListener("resize", () => {
        PositionCommandCompletionWindow();
    });
}

function addGridPaginationHandlers() {
    UI.LeftPaginationButton.addEventListener("click", () => {
        DataGrid.PageDown();
    });

    UI.RightPaginationButton.addEventListener("click", () => {
        DataGrid.PageUp();
    });

    var changePageTimeout = -1;
    UI.CurrentPageInput.addEventListener("input", () => {
        if (changePageTimeout > 0) {
            window.clearTimeout(changePageTimeout);
        }
        
        changePageTimeout = window.setTimeout(() => {
            DataGrid.GoToCurrentPage();
        }, 1500);
    });
}

function addAlertHandlers() {
    UI.AlertsButton.addEventListener("click", () => {
        UI.AlertsFrame.classList.toggle("open");
    });
    UI.CloseAlertsButton.addEventListener("click", () => {
        UI.AlertsFrame.classList.toggle("open");
    });

    UI.ClearAllAlertsButton.addEventListener("click", () => {
        var result = confirm("Are you sure you want to delete all alerts?");
        if (result) {
            var xhr = new XMLHttpRequest();
            xhr.open("delete", location.origin + "/api/Alerts/DeleteAll/");
            xhr.onload = function () {
                if (xhr.status == 200) {
                    UI.AlertsBody.innerHTML = "";
                    UI.AlertsCount.innerText = "0";
                }
                else {
                    ShowModal("API Error", "There was an error deleting the alerts.");
                }
            };
            xhr.send();
        }
    });

    document.querySelectorAll(".alert-dismiss-button").forEach(element => {
        element.addEventListener("click", ev => {
            var alertID = (ev.currentTarget as HTMLButtonElement).getAttribute("alert");
            var xhr = new XMLHttpRequest();
            xhr.open("delete", location.origin + "/api/Alerts/Delete/" + alertID);
            xhr.onload = function () {
                if (xhr.status == 200) {
                    document.getElementById(alertID).remove();
                    var currentCount = Number(UI.AlertsCount.innerText);
                    currentCount--;
                    UI.AlertsCount.innerText = String(currentCount);
                }
                else {
                    ShowModal("API Error", "There was an error deleting the alert.");
                }
            };
            xhr.send();
        })
    });
}

function arrowUpOrDownOnTextArea(e: KeyboardEvent) {
    if (e.ctrlKey) {
        if (e.key.toLowerCase() == "arrowdown") {
            UI.TabContentWrapper.scrollTop += 30;
        }
        else if (e.key.toLowerCase() == "arrowup") {
            UI.TabContentWrapper.scrollTop -= 30;
        }
    }
    else {
        if (!UI.CommandCompletionDiv.classList.contains("hidden")) {
            if (e.key.toLowerCase() == "arrowdown") {
                if (CommandCompletionStore.CompletionPosition < UI.CommandCompletionDiv.children.length - 1) {
                    CommandCompletionStore.CompletionPosition += 1;
                    HighlightCompletionWindowItem(CommandCompletionStore.CompletionPosition);
                    (UI.CommandCompletionDiv.querySelector(".selected") as HTMLElement).onfocus(new FocusEvent(""));
                }
            }
            else if (e.key.toLowerCase() == "arrowup") {
                if (CommandCompletionStore.CompletionPosition > 0) {
                    CommandCompletionStore.CompletionPosition -= 1;
                    HighlightCompletionWindowItem(CommandCompletionStore.CompletionPosition);
                    (UI.CommandCompletionDiv.querySelector(".selected") as HTMLElement).onfocus(new FocusEvent(""));
                }
            }
        }
        else {
            if (e.key.toLowerCase() == "arrowdown") {
                if (CommandCompletionStore.InputHistoryPosition < CommandCompletionStore.InputHistoryItems.length - 1) {
                    CommandCompletionStore.InputHistoryPosition += 1;
                    UI.ConsoleTextArea.value = CommandCompletionStore.InputHistoryItems[CommandCompletionStore.InputHistoryPosition];
                }
            }
            else if (e.key.toLowerCase() == "arrowup") {
                if (CommandCompletionStore.InputHistoryPosition > 0) {
                    CommandCompletionStore.InputHistoryPosition -= 1;
                    UI.ConsoleTextArea.value = CommandCompletionStore.InputHistoryItems[CommandCompletionStore.InputHistoryPosition];
                }
            }
        }
    }
}

function clickToggleAllDevices() {
    document.getElementById("toggleAllDevices").addEventListener("click", function () {
        DataGrid.ToggleSelectAll();
    })
}

function clickStartRemoteControlButton() {
    document.getElementById("startRemoteControlButton").addEventListener("click", function () {
        var selectedDevices = DataGrid.GetSelectedDevices();
        if (selectedDevices.length == 0) {
            ShowMessage("You must select a device first.");
        }
        else if (selectedDevices.length > 1) {
            ShowMessage("You must select only one device to control.");
        }
        else {
            BrowserHubConnection.StartRemoteControl(selectedDevices[0].ID, false);
        }
    })
}

function consoleTabSelected() {
    $(UI.ConsoleTab).on("shown.bs.tab", () => {
        UI.ConsoleAlert.hidden = true;
        UI.ConsoleAlert.innerText = "0";
        UI.ConsoleFrame.scrollTop = UI.ConsoleFrame.scrollHeight;
    });
}

function deviceGroupSelectChanged() {
    UI.DeviceGroupSelect.addEventListener("change", () => {
        DataGrid.GridState.GroupFilter = UI.DeviceGroupSelect.value;
        if (UI.DeviceGroupSelect.selectedIndex == 0) {
            DataGrid.GridState.ShowAllGroups = true;
        }
        else {
            DataGrid.GridState.ShowAllGroups = false;
        }
        DataGrid.ApplyFilterToAll();
    });
}

function hideOfflineDevicesCheckboxChanged() {
    UI.HideOfflineDevicesCheckbox.addEventListener("change", () => {
        DataGrid.GridState.HideOffline = UI.HideOfflineDevicesCheckbox.checked;
        DataGrid.ApplyFilterToAll();
    });
}

function inputOnCommandTextArea() {
    UI.ConsoleTextArea.addEventListener("input", () => {
        var commandMode = CommandProcessor.GetCommandModeShortcut();
        if (commandMode) {
            UI.CommandModeSelect.value = commandMode;
            UI.ConsoleTextArea.value = "";
            UI.CommandCompletionDiv.classList.add("hidden");
        }
        else {
            CommandProcessor.EvaluateCurrentCommandText();
        }
        UI.ConsoleFrame.scrollTop = UI.ConsoleFrame.scrollHeight;
    });
}

function inputOnFilterTextBox() {
    UI.GridFilter.addEventListener("input", (e) => {
        var currentText = (e.currentTarget as HTMLInputElement).value.toLowerCase();
        DataGrid.GridState.SearchFilter = currentText;
        DataGrid.ApplyFilterToAll();
    })
}

function keyDownOnInputTextArea() {
    UI.ConsoleTextArea.addEventListener("keydown", function (e: KeyboardEvent) {
        if (!e.shiftKey) {
            switch (e.key.toLowerCase()) {
                case "enter":
                    e.preventDefault();
                    if (UI.ConsoleTextArea.value.trim().length == 0) {
                        return;
                    }
                    UI.CommandCompletionDiv.classList.add("hidden");
                    UI.CommandInfoDiv.classList.add("hidden");
                    AddConsoleHTML("span", "echo-input", UI.ConsoleTextArea.value);
                    if (!BrowserHubConnection.Connected) {
                        AddConsoleOutput("Not connected.  Reconnecting...");
                        BrowserHubConnection.Connect();
                        return;
                    }
                    CommandProcessor.ProcessCommand();
                    break;
                case "arrowup":
                case "arrowdown":
                    e.preventDefault();
                    arrowUpOrDownOnTextArea(e);
                    break;
                case "escape":
                    if (!UI.CommandCompletionDiv.classList.contains("hidden")) {
                        e.preventDefault();
                        UI.CommandCompletionDiv.classList.add("hidden");
                        UI.CommandInfoDiv.classList.add("hidden");
                    }
                    else {
                        e.preventDefault();
                        UI.ConsoleTextArea.value = "";
                    }
                    break;
                case "tab":
                    if (!UI.CommandCompletionDiv.classList.contains("hidden")) {
                        e.preventDefault();
                        (UI.CommandCompletionDiv.querySelector(".selected") as HTMLElement).click();
                    }
                    break;
                case "backspace":
                    if (UI.ConsoleTextArea.value.length == 0 && !UI.CommandCompletionDiv.classList.contains("hidden")) {
                        UI.CommandCompletionDiv.classList.add("hidden");
                        UI.CommandInfoDiv.classList.add("hidden");
                    }
                    break;
                default:
                    break;
            }
        }
    })
}

function keyDownOnWindow() {
    window.addEventListener("keydown", (e: KeyboardEvent) => {
        if (!document.activeElement.isEqualNode(UI.ConsoleTextArea) &&
            document.activeElement.tagName.toLowerCase() != "select" &&
            document.activeElement.tagName.toLowerCase() != "input" &&
            document.activeElement.tagName.toLowerCase() != "textarea" &&
            !e.altKey &&
            !e.ctrlKey) {
            UI.ConsoleTextArea.focus();
        }
        if (e.ctrlKey && e.key.toLowerCase() == "q") {
            UI.ConsoleOutputDiv.innerHTML = "";
        }
    });
}
