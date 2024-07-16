import { DtoType } from "./Enums/DtoType.js";
import { DtoWrapper } from "./Interfaces/Dtos.js";
import { MessagePack } from "./Interfaces/MessagePack.js";
import { CreateGUID } from "./Utilities.js";

const Chunks: Record<string, DtoWrapper[]> = {};
const MsgPack: MessagePack = window["MessagePack"];

export function ChunkDto<T>(dto: T, dtoType: DtoType, requestId = null, chunkSize = 50000) : DtoWrapper[] {
    const messageBytes = MsgPack.encode(dto);
    const instanceId = CreateGUID();
    const chunks: Array<Uint8Array> = [];
    const wrappers: Array<DtoWrapper> = [];
    
    for (let i = 0; i < messageBytes.length; i += chunkSize) {

        const chunk = messageBytes.subarray(i, i + chunkSize);
        chunks.push(chunk);
    }

    for (let i = 0; i < chunks.length; i++) {
        const chunk = chunks[i];

        const wrapper = new DtoWrapper();
        wrapper.DtoChunk = chunk;
        wrapper.DtoType = dtoType;
        wrapper.InstanceId = instanceId;
        wrapper.IsFirstChunk = i == 0;
        wrapper.IsLastChunk = i == chunks.length - 1;
        wrapper.RequestId = requestId;
        wrapper.SequenceId = i;

        wrappers.push(wrapper);
    }

    return wrappers;
}


export function TryComplete<T>(wrapper: DtoWrapper) : T {
    if (!Chunks[wrapper.InstanceId]) {
        Chunks[wrapper.InstanceId] = [];
    }

    Chunks[wrapper.InstanceId].push(wrapper);

    if (!wrapper.IsLastChunk) {
        return;
    }

    const buffers = Chunks[wrapper.InstanceId]
        .sort((a, b) => a.SequenceId - b.SequenceId)
        .map(x => x.DtoChunk)
        .reduce((prev,cur) => new Uint8Array([...prev, ...cur]));

    delete Chunks[wrapper.InstanceId];

    var decoded = MsgPack.decode<T>(buffers);
    return decoded as T;
}
