import type { EncoderOptions } from "./Encoder";
import type { SplitUndefined } from "./context";
/**
 * It encodes `value` in the MessagePack format and
 * returns a byte buffer.
 *
 * The returned buffer is a slice of a larger `ArrayBuffer`, so you have to use its `#byteOffset` and `#byteLength` in order to convert it to another typed arrays including NodeJS `Buffer`.
 */
export declare function encode<ContextType = undefined>(value: unknown, options?: EncoderOptions<SplitUndefined<ContextType>>): Uint8Array;
