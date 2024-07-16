export interface MessagePack {
    encode<T>(item: T): Uint8Array;
    decode<T>(message: Uint8Array) : T;
    decode<T>(message: ArrayBuffer) : T;
}