import { MainRc } from "./Main.js";
export class ClipboardWatcher {
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
                        MainRc.MessageSender.SendClipboardTransfer(newText, false);
                    }
                });
            }, 100);
        });
    }
    SetClipboardText(text) {
        if (text == this.LastClipboardText) {
            return;
        }
        this.PauseMonitoring = true;
        this.LastClipboardText = text;
        navigator.clipboard.writeText(text);
        this.PauseMonitoring = false;
    }
}
//# sourceMappingURL=ClipboardWatcher.js.map