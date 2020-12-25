import { UserSettings } from "./UserSettings.js";
import { ConsoleOutputDiv, ConsoleFrame, ConsoleTab, ConsoleAlert, ConsoleTextArea } from "./UI.js";
import { EncodeForHTML } from "../Shared/Utilities.js";

export function AddConsoleOutput(strOutputMessage: string) {
    var outputBlock = document.createElement("div");
    outputBlock.classList.add("console-block");

    var prompt = document.createElement("div");
    prompt.classList.add("console-prompt");
    prompt.innerText = UserSettings.PromptString;

    var output = document.createElement("div");
    output.classList.add("console-output");
    output.innerText = strOutputMessage;

    outputBlock.appendChild(prompt);
    outputBlock.appendChild(output);

    ConsoleOutputDiv.appendChild(outputBlock);

    ConsoleFrame.scrollTop = ConsoleFrame.scrollHeight;

    IncrementMissedMessageCount();
}

export function AddConsoleHTML(elementTag: string, className: string, content: string, extraContent: string = null) {
    var innerEle = document.createElement(elementTag);
    innerEle.className = className;
    innerEle.innerText = content;

    var contentWrapper = document.createElement("div");
    contentWrapper.appendChild(innerEle);

    if (extraContent) {
        var extra = document.createElement("span");
        extra.innerText = extraContent;
        contentWrapper.appendChild(extra);
    }

    ConsoleOutputDiv.appendChild(contentWrapper);

    ConsoleFrame.scrollTop = ConsoleFrame.scrollHeight;

    IncrementMissedMessageCount();
}

export function AddConsoleTrustedHtml(trustedHtml: string) {
    var contentWrapper = document.createElement("div");
    contentWrapper.innerHTML = trustedHtml;
    ConsoleOutputDiv.appendChild(contentWrapper);

    ConsoleFrame.scrollTop = ConsoleFrame.scrollHeight;

    IncrementMissedMessageCount();
}
export function AddConsoleLineBreak(count: number = 1) {
    for (var i = 0; i < count; i++) {
        ConsoleOutputDiv.appendChild(document.createElement("br"));
    }
}
export function AddConsoleElement(element: HTMLElement) {
    ConsoleOutputDiv.appendChild(element);
    ConsoleFrame.scrollTop = ConsoleFrame.scrollHeight;
    IncrementMissedMessageCount();
}
export function AddTransferHarness(transferID: string, totalDevices: number) {
    var transferHarness = document.createElement("div");
    transferHarness.id = transferID;
    transferHarness.classList.add("command-harness");
    transferHarness.innerHTML = `
        <div class="command-harness-title">
            File Transfer Status  |  
            Total Devices: ${totalDevices} |  
            Completed: <span id="${transferID}-completed">0</span>
        </div>`;
    AddConsoleElement(transferHarness);
}
export function AutoSizeTextArea() {
    ConsoleTextArea.style.height = "1px";
    ConsoleTextArea.style.height = Math.max(12, ConsoleTextArea.scrollHeight) + "px";
}

export function IncrementMissedMessageCount() {
    if (!ConsoleTab.classList.contains("active")) {
        var currentCount = Number.parseInt(ConsoleAlert.innerText);
        if (Number.isNaN(currentCount)) {
            ConsoleAlert.innerText = "1";
        }
        else {
            ConsoleAlert.innerText = String(currentCount + 1);
        }

        ConsoleAlert.hidden = false;
    }
}