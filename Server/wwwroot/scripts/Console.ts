import { UserSettings } from "./UserSettings.js";
import { ConsoleOutputDiv, ConsoleFrame, ConsoleTab, ConsoleAlert, ConsoleTextArea } from "./UI.js";

export function AddConsoleOutput(strOutputMessage: string) {
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

    IncrementMissedMessageCount();
}
export function AddConsoleHTML(html: string) {
    var contentWrapper = document.createElement("div");
    contentWrapper.innerHTML = html;
    ConsoleOutputDiv.appendChild(contentWrapper);

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
    AddConsoleHTML(transferHarness.outerHTML);
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