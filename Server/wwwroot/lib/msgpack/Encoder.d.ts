import { ExtensionCodecType } from "./ExtensionCodec";
import type { ContextOf } from "./context";
export declare const DEFAULT_MAX_DEPTH = 100;
export declare const DEFAULT_INITIAL_BUFFER_SIZE = 2048;
export type EncoderOptions<ContextType = undefined> = Partial<Readonly<{
    extensionCodec: ExtensionCodecType<ContextType>;
    /**
     * Encodes bigint as Int64 or Uint64 if it's set to true.
     * {@link forceIntegerToFloat} does not affect bigint.
     * Depends on ES2020's {@link DataView#setBigInt64} and
     * {@link DataView#setBigUint64}.
     *
     * Defaults to false.
     */
    useBigInt64: boolean;
    /**
     * The maximum depth in nested objects and arrays.
     *
     * Defaults to 100.
     */
    maxDepth: number;
    /**
     * The initial size of the internal buffer.
     *
     * Defaults to 2048.
     */
    initialBufferSize: number;
    /**
     * If `true`, the keys of an object is sorted. In other words, the encoded
     * binary is canonical and thus comparable to another encoded binary.
     *
     * Defaults to `false`. If enabled, it spends more time in encoding objects.
     */
    sortKeys: boolean;
    /**
     * If `true`, non-integer numbers are encoded in float32, not in float64 (the default).
     *
     * Only use it if precisions don't matter.
     *
     * Defaults to `false`.
     */
    forceFloat32: boolean;
    /**
     * If `true`, an object property with `undefined` value are ignored.
     * e.g. `{ foo: undefined }` will be encoded as `{}`, as `JSON.stringify()` does.
     *
     * Defaults to `false`. If enabled, it spends more time in encoding objects.
     */
    ignoreUndefined: boolean;
    /**
     * If `true`, integer numbers are encoded as floating point numbers,
     * with the `forceFloat32` option taken into account.
     *
     * Defaults to `false`.
     */
    forceIntegerToFloat: boolean;
}>> & ContextOf<ContextType>;
export declare class Encoder<ContextType = undefined> {
    private readonly extensionCodec;
    private readonly context;
    private readonly useBigInt64;
    private readonly maxDepth;
    private readonly initialBufferSize;
    private readonly sortKeys;
    private readonly forceFloat32;
    private readonly ignoreUndefined;
    private readonly forceIntegerToFloat;
    private pos;
    private view;
    private bytes;
    private entered;
    constructor(options?: EncoderOptions<ContextType>);
    private clone;
    private reinitializeState;
    private enteringGuard;
    /**
     * This is almost equivalent to {@link Encoder#encode}, but it returns an reference of the encoder's internal buffer and thus much faster than {@link Encoder#encode}.
     *
     * @returns Encodes the object and returns a shared reference the encoder's internal buffer.
     */
    encodeSharedRef(object: unknown): Uint8Array;
    /**
     * @returns Encodes the object and returns a copy of the encoder's internal buffer.
     */
    encode(object: unknown): Uint8Array;
    private doEncode;
    private ensureBufferSizeToWrite;
    private resizeBuffer;
    private encodeNil;
    private encodeBoolean;
    private encodeNumber;
    private encodeNumberAsFloat;
    private encodeBigInt64;
    private writeStringHeader;
    private encodeString;
    private encodeObject;
    private encodeBinary;
    private encodeArray;
    private countWithoutUndefined;
    private encodeMap;
    private encodeExtension;
    private writeU8;
    private writeU8a;
    private writeI8;
    private writeU16;
    private writeI16;
    private writeU32;
    private writeI32;
    private writeF32;
    private writeF64;
    private writeU64;
    private writeI64;
    private writeBigUint64;
    private writeBigInt64;
}
