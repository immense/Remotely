import { GetCurrentViewer } from "./UI.js";
export class SessionRecorder {
    constructor() {
        this.RecordedData = [];
    }
    Start() {
        if (!window["MediaRecorder"] || !GetCurrentViewer().captureStream) {
            alert("Session recording isn't supported on this browser.");
            return;
        }
        if (this.Recorder && this.Recorder.state != "inactive") {
            this.Recorder.stop();
        }
        this.RecordedData = [];
        this.Stream = GetCurrentViewer().captureStream(10);
        var options = { mimeType: 'video/webm' };
        this.Recorder = new window["MediaRecorder"](this.Stream, options);
        this.Recorder.ondataavailable = (event) => {
            if (event.data && event.data.size > 0) {
                this.RecordedData.push(event.data);
            }
        };
        this.Recorder.start(100);
    }
    Stop() {
        if (!this.Recorder) {
            return;
        }
        this.Recorder.stop();
    }
    DownloadVideo() {
        if (!this.RecordedData || this.RecordedData.length == 0) {
            alert("No video recorded.");
            return;
        }
        if (this.Recorder && this.Recorder.state != "inactive") {
            alert("You must stop recording before you can download.");
            return;
        }
        var currentDate = new Date();
        var dateString = `${currentDate.getFullYear()}-` +
            `${(currentDate.getMonth() + 1).toString().padStart(2, "0")}-` +
            `${currentDate.getDate().toString().padStart(2, "0")} ` +
            `${currentDate.getHours().toString().padStart(2, "0")}.` +
            `${currentDate.getMinutes().toString().padStart(2, "0")}.` +
            `${currentDate.getSeconds().toString().padStart(2, "0")}`;
        var blob = new Blob(this.RecordedData, { type: 'video/webm' });
        var url = window.URL.createObjectURL(blob);
        var link = document.createElement('a');
        link.style.display = 'none';
        link.href = url;
        link.download = `Remote_Session_${dateString}.webm`;
        document.body.appendChild(link);
        link.click();
        setTimeout(() => {
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        }, 100);
    }
}
//# sourceMappingURL=SessionRecorder.js.map