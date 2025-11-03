export declare const EXT_TIMESTAMP = -1;
export type TimeSpec = {
    sec: number;
    nsec: number;
};
export declare function encodeTimeSpecToTimestamp({ sec, nsec }: TimeSpec): Uint8Array;
export declare function encodeDateToTimeSpec(date: Date): TimeSpec;
export declare function encodeTimestampExtension(object: unknown): Uint8Array | null;
export declare function decodeTimestampToTimeSpec(data: Uint8Array): TimeSpec;
export declare function decodeTimestampExtension(data: Uint8Array): Date;
export declare const timestampExtension: {
    type: number;
    encode: typeof encodeTimestampExtension;
    decode: typeof decodeTimestampExtension;
};
