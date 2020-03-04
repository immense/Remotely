import { ClipboardTransferTextArea } from "./UI.js";
import { RemoteControl } from "./Main.js";
var clipboardTimer = -1;
var lastClipboardText = "";
export function WatchClipboard() {
    navigator.clipboard.readText().then(currentText => {
        lastClipboardText = currentText;
        clipboardTimer = setInterval(() => {
            navigator.clipboard.readText().then(newText => {
                if (lastClipboardText != newText) {
                    lastClipboardText = newText;
                    ClipboardTransferTextArea.value = newText;
                    RemoteControl.RCBrowserSockets.SendClipboardTransfer(newText, false);
                }
            });
        }, 100);
    });
}
//# sourceMappingURL=ClipboardWatcher.js.map