export const Sound = new class {
    constructor() {
        this.AudioElements = new Array();
        this.SourceNodes = new Array();
        if (AudioContext) {
            this.Context = new AudioContext();
        }
        else if (window["webkitAudioContext"]) {
            this.Context = new window["webkitAudioContext"];
        }
        else {
            return;
        }
        this.BackgroundAudio = new Audio();
        this.BackgroundNode = this.Context.createMediaElementSource(this.BackgroundAudio);
        this.BackgroundNode.connect(this.Context.destination);
    }
    Play(buffer) {
        if (!this.Context) {
            return;
        }
        var fr = new FileReader();
        fr.onload = async (ev) => {
            var audioBuffer = await this.Context.decodeAudioData(fr.result);
            var bufferSource = this.Context.createBufferSource();
            bufferSource.buffer = audioBuffer;
            bufferSource.connect(this.Context.destination);
            bufferSource.start();
        };
        fr.readAsArrayBuffer(new Blob([buffer], { 'type': 'audio/wav' }));
    }
    ;
};
//# sourceMappingURL=Sound.js.map