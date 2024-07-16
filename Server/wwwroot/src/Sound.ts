export const Sound = new class {
    private _context: AudioContext;
    private _fileReader: FileReader = new FileReader();

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

        this._fileReader.onload
    }

    async Play(buffer: Uint8Array) {
        if (!this._context) {
            return;
        }

        let audioBuffer = await this.GetAudioBuffer(buffer);

        let bufferSource = this._context.createBufferSource();
        bufferSource.buffer = audioBuffer;
        bufferSource.connect(this._context.destination);
        bufferSource.start();
    };

    private GetAudioBuffer(buffer: Uint8Array): Promise<AudioBuffer> {
        return new Promise<AudioBuffer>((resolve, reject) => {
            try {
                let fr = new FileReader();
                fr.onload = async (ev) => {
                    try {
                        let audioBuffer = await this._context.decodeAudioData(fr.result as ArrayBuffer);
                        resolve(audioBuffer);
                    }
                    catch (ex) {
                        reject(ex);
                    }
                }
                fr.readAsArrayBuffer(new Blob([buffer], { 'type': 'audio/wav' }));
            }
            catch (ex) {
                reject(ex);
            }
        });
    }
}