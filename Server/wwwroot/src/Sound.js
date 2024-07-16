export const Sound = new class {
    constructor() {
        this._fileReader = new FileReader();
    }
    Init() {
        if (this._context) {
            // Already initialized.
            return;
        }
        if (AudioContext) {
            this._context = new AudioContext();
        }
        else if (window["webkitAudioContext"]) {
            this._context = new window["webkitAudioContext"];
        }
        this._fileReader.onload;
    }
    async Play(buffer) {
        if (!this._context) {
            return;
        }
        let audioBuffer = await this.GetAudioBuffer(buffer);
        let bufferSource = this._context.createBufferSource();
        bufferSource.buffer = audioBuffer;
        bufferSource.connect(this._context.destination);
        bufferSource.start();
    }
    ;
    GetAudioBuffer(buffer) {
        return new Promise((resolve, reject) => {
            try {
                let fr = new FileReader();
                fr.onload = async (ev) => {
                    try {
                        let audioBuffer = await this._context.decodeAudioData(fr.result);
                        resolve(audioBuffer);
                    }
                    catch (ex) {
                        reject(ex);
                    }
                };
                fr.readAsArrayBuffer(new Blob([buffer], { 'type': 'audio/wav' }));
            }
            catch (ex) {
                reject(ex);
            }
        });
    }
};
//# sourceMappingURL=Sound.js.map