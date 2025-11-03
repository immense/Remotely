export interface KeyDecoder {
    canBeCached(byteLength: number): boolean;
    decode(bytes: Uint8Array, inputOffset: number, byteLength: number): string;
}
export declare class CachedKeyDecoder implements KeyDecoder {
    hit: number;
    miss: number;
    private readonly caches;
    private readonly maxKeyLength;
    private readonly maxLengthPerKey;
    constructor(maxKeyLength?: number, maxLengthPerKey?: number);
    canBeCached(byteLength: number): boolean;
    private find;
    private store;
    decode(bytes: Uint8Array, inputOffset: number, byteLength: number): string;
}
