import { ViewerApp } from "./App.js";
export class ClipboardWatcher {
    WatchClipboard() {
        if (navigator.clipboard.readText) {
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
                        ViewerApp.MessageSender.SendClipboardTransfer(newText, false);
                    }
                });
            }, 100);
        }
    }
    SetClipboardText(text) {
        if (text == this.LastClipboardText) {
            return;
        }
        if (navigator.clipboard.writeText) {
            this.PauseMonitoring = true;
            this.LastClipboardText = text;
            navigator.clipboard.writeText(text);
            this.PauseMonitoring = false;
        }
    }
}
//# sourceMappingURL=ClipboardWatcher.js.map