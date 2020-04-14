import { ClipboardTransferTextArea } from "./UI.js";
import { MainRc } from "./Main.js";

export class ClipboardWatcher {
    ClipboardTimer: number;
    LastClipboardText: string;
    PauseMonitoring: boolean;

    WatchClipboard() {
        navigator.clipboard.readText().then(currentText => {
            this.LastClipboardText = currentText;

            this.ClipboardTimer = setInterval(() => {
                if (this.PauseMonitoring) {
                    return;
                }
                if (!document.hasFocus()) {
                    return;
                }

                navigator.clipboard.readText().then(newText => {
                    if (this.LastClipboardText != newText) {
                        this.LastClipboardText = newText;
                        ClipboardTransferTextArea.value = newText;
                        MainRc.RCBrowserSockets.SendClipboardTransfer(newText, false);
                    }
                })
            }, 100);
        });
    }
    
    SetClipboardText(text: string) {
        this.PauseMonitoring = true;
        this.LastClipboardText = text;
        navigator.clipboard.writeText(text);
        ClipboardTransferTextArea.value = text;
        this.PauseMonitoring = false;
    }
}