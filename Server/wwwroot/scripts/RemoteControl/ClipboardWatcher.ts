import { ClipboardTransferTextArea } from "./UI.js";
import { Remotely } from "./Main.js";

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
                    Remotely.RCBrowserSockets.SendClipboardTransfer(newText, false);
                }
            })
        }, 100);
    });
}