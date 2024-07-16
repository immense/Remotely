import { ScreenViewer } from "./UI.js";
import { ViewerApp } from "./App.js";

export class SessionRecorder {
    private recorder: MediaRecorder;
    private subject: any;

    async Start() {
        if (!window["MediaRecorder"] || !ScreenViewer.captureStream) {
            console.warn("Session recording isn't supported on this browser.");
            return;
        }

        if (this.recorder && this.recorder.state != "inactive") {
            this.recorder.stop();
        }

        const stream = ScreenViewer.captureStream(10);
        const options = { mimeType: 'video/webm' };
        this.subject = new window["signalR"].Subject();
        await ViewerApp.ViewerHubConnection.SendRecordingChunks(this.subject);

        this.recorder = new MediaRecorder(stream, options);
        
        this.recorder.ondataavailable = async (event) => {
            if (!event.data || event.data.size <= 0) {
                return
            }

            const buffer = await event.data.arrayBuffer();

            for (let i = 0; i < buffer.byteLength; i += 50_000) {

                const chunk = buffer.slice(i, i + 50_000);
                const byteArray = new Uint8Array(chunk);
                this.subject.next(byteArray);
            }
        }
        this.recorder.start(100);
    }

    Stop() {
        this.recorder?.stop();
        this.subject?.complete();
    }

}