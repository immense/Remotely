import { PositionCommandCompletionWindow, HighlightCompletionWindowItem } from "./CommandCompletion.js";
import * as UI from "./UI.js";
import * as CommandProcessor from "./CommandProcessor.js";
import { Store } from "./Store.js";
import * as DataGrid from "./DataGrid.js";
import * as HubConnection from "./HubConnection.js";
import { WebCommands } from "./Commands/WebCommands.js";
import { AddConsoleOutput } from "./Console.js";


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

    window.addEventListener("resize", ev => {
        PositionCommandCompletionWindow();
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
                if (Store.CommandCompletionPosition < UI.CommandCompletionDiv.children.length - 1) {
                    Store.CommandCompletionPosition += 1;
                    HighlightCompletionWindowItem(Store.CommandCompletionPosition);
                    (UI.CommandCompletionDiv.querySelector(".selected") as HTMLElement).onfocus(new FocusEvent(""));
                }
            }
            else if (e.key.toLowerCase() == "arrowup") {
                if (Store.CommandCompletionPosition > 0) {
                    Store.CommandCompletionPosition -= 1;
                    HighlightCompletionWindowItem(Store.CommandCompletionPosition);
                    (UI.CommandCompletionDiv.querySelector(".selected") as HTMLElement).onfocus(new FocusEvent(""));
                }
            }
        }
        else {
            if (e.key.toLowerCase() == "arrowdown") {
                if (Store.InputHistoryPosition < Store.InputHistoryItems.length - 1) {
                    Store.InputHistoryPosition += 1;
                    UI.ConsoleTextArea.value = Store.InputHistoryItems[Store.InputHistoryPosition];
                }
            }
            else if (e.key.toLowerCase() == "arrowup") {
                if (Store.InputHistoryPosition > 0) {
                    Store.InputHistoryPosition -= 1;
                    UI.ConsoleTextArea.value = Store.InputHistoryItems[Store.InputHistoryPosition];
                }
            }
        }
    }
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
                    AddConsoleOutput(`<span class="echo-input">${UI.ConsoleTextArea.value}</span>`);
                    if (!HubConnection.Connected) {
                        AddConsoleOutput("Not connected.  Reconnecting...");
                        HubConnection.Connect();
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

function inputOnCommandTextArea() {
    UI.ConsoleTextArea.addEventListener("input", (e: KeyboardEvent) => {
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
        DataGrid.FilterOptions.SearchFilter = currentText;
        DataGrid.ApplyFilter();
    })
}
function consoleTabSelected() {
    $(UI.ConsoleTab).on("shown.bs.tab", () => {
        UI.ConsoleAlert.hidden = true;
        UI.ConsoleFrame.scrollTop = UI.ConsoleFrame.scrollHeight;
    });
}
function addAlertHandlers() {
    UI.AlertsButton.addEventListener("click", ev => {
        UI.AlertsFrame.classList.toggle("open");
    });
    UI.CloseAlertsButton.addEventListener("click", ev => {
        UI.AlertsFrame.classList.toggle("open");
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
                    UI.ShowModal("API Error", "There was an error deleting the alert.");
                }
            };
            xhr.send();
        })
    });
}
function clickToggleAllDevices() {
    document.getElementById("toggleAllDevices").addEventListener("click", function (e) {
        DataGrid.ToggleSelectAll();
    })
}

function clickStartRemoteControlButton() {
    document.getElementById("startRemoteControlButton").addEventListener("click", function (e) {
        var selectedDevices = DataGrid.GetSelectedDevices();
        if (selectedDevices.length == 0) {
            UI.ShowMessage("You must select a device first.");
        }
        else if (selectedDevices.length > 1) {
            UI.ShowMessage("You must select only one device to control.");
        }
        else {
            WebCommands.find(x => x.Name == "RemoteControl").Execute([]);
        }
    })
}

function deviceGroupSelectChanged() {
    UI.DeviceGroupSelect.addEventListener("change", (ev) => {
        DataGrid.FilterOptions.GroupFilter = UI.DeviceGroupSelect.value;
        if (UI.DeviceGroupSelect.selectedIndex == 0) {
            DataGrid.FilterOptions.ShowAllGroups = true;
        }
        else {
            DataGrid.FilterOptions.ShowAllGroups = false;
        }
        DataGrid.ApplyFilter();
    });
}