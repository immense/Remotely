import { ViewerApp } from "./App.js";
import { ShowMessage } from "./UI.js";

export class ClipboardWatcher {
    ClipboardTimer: number;
    LastClipboardText: string;
    NewClipboardText: string;

    WatchClipboard() {
        if (navigator.clipboard.readText) {

            this.ClipboardTimer = setInterval(() => {
                if (!document.hasFocus()) {
                    return;
                }

                if (this.NewClipboardText && navigator.clipboard.writeText) {
                    navigator.clipboard.writeText(this.NewClipboardText);
                    this.LastClipboardText = this.NewClipboardText;
                    this.NewClipboardText = null;
                    ShowMessage("Clipboard updated.");
                    return;
                }

                navigator.clipboard.readText().then(newText => {
                    if (this.LastClipboardText != newText) {
                        this.LastClipboardText = newText;
                        ViewerApp.MessageSender.SendClipboardTransfer(newText, false);
                    }
                })
            }, 500);
        }
    }
    
    SetClipboardText(text: string) {
        if (text == this.LastClipboardText) {
            return;
        }

        this.NewClipboardText = text;
    }
}