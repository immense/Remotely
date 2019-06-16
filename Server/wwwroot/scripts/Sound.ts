import { RemoveFromArray } from "./Utilities.js";

export const Sound = new class {
    constructor() {
        this.Context = new AudioContext();
        this.BackgroundAudio = new Audio();
        this.BackgroundNode = this.Context.createMediaElementSource(this.BackgroundAudio);
        this.BackgroundNode.connect(this.Context.destination);
    }
    Context: AudioContext;
    AudioElements: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
    SourceNodes: Array<MediaElementAudioSourceNode> = new Array<MediaElementAudioSourceNode>();

    BackgroundAudio: HTMLAudioElement;
    BackgroundNode: MediaElementAudioSourceNode;

    Play(buffer: Uint8Array) {

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