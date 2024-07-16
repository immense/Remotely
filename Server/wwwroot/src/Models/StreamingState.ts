export class StreamingState {
    constructor() {
        this.Buffer = new Blob();
        this.ReceivedChunks = [];
        this.StreamEnded = false;
    }

    Buffer: Blob;
    ReceivedChunks: Uint8Array[];
    StreamEnded: boolean;
}