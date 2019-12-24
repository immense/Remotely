import { UserSettings } from "./UserSettings.js";
import { ConsoleOutputDiv, ConsoleFrame, ConsoleTab, ConsoleAlert, ConsoleTextArea } from "./UI.js";
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
//# sourceMappingURL=Console.js.map