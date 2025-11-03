function toPlainArrayBuffer(u8) {
    const ab = new ArrayBuffer(u8.byteLength);
    new Uint8Array(ab).set(u8);
    return ab;
}
export const Sound = new class {
    constructor() {
        this._fileReader = new FileReader();
    }
    Init() {
        if (this._context)
            return;
        if (typeof AudioContext !== "undefined") {
            this._context = new AudioContext();
        }
        else if (window["webkitAudioContext"]) {
            this._context = new window["webkitAudioContext"]();
        }
    }
    async Play(buffer) {
        if (!this._context)
            return;
        const audioBuffer = await this.GetAudioBuffer(buffer);
        const src = this._context.createBufferSource();
        src.buffer = audioBuffer;
        src.connect(this._context.destination);
        src.start();
    }
    GetAudioBuffer(buffer) {
        return new Promise((resolve, reject) => {
            try {
                const fr = new FileReader();
                fr.onload = async () => {
                    try {
                        const audioBuffer = await this._context.decodeAudioData(fr.result);
                        resolve(audioBuffer);
                    }
                    catch (ex) {
                        reject(ex);
                    }
                };
                // Use a plain ArrayBuffer BlobPart (not SharedArrayBuffer)
                const part = toPlainArrayBuffer(buffer);
                fr.readAsArrayBuffer(new Blob([part], { type: "audio/wav" }));
            }
            catch (ex) {
                reject(ex);
            }
        });
    }
}();
//# sourceMappingURL=Sound.js.map