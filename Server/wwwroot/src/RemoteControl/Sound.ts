export const Sound = new class {
    Context: AudioContext;

    Init() {
        if (this.Context) {
            // Already initialized.
            return;
        }

        if (AudioContext) {
            this.Context = new AudioContext();
        }
        else if (window["webkitAudioContext"]) {
            this.Context = new window["webkitAudioContext"];
        }
        else {
            return;
        }
    }

    Play(buffer: Uint8Array) {
        if (!this.Context) {
            return;
        }

        var fr = new FileReader();
        fr.onload = async (ev) => {
            var audioBuffer = await this.Context.decodeAudioData(fr.result as ArrayBuffer);
            var bufferSource = this.Context.createBufferSource();
            bufferSource.buffer = audioBuffer;
            bufferSource.connect(this.Context.destination);
            bufferSource.start();
        }

        fr.readAsArrayBuffer(new Blob([buffer], { 'type': 'audio/wav' }));
    };
}