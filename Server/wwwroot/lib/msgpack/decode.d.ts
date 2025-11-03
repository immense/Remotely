import type { DecoderOptions } from "./Decoder";
import type { SplitUndefined } from "./context";
/**
 * It decodes a single MessagePack object in a buffer.
 *
 * This is a synchronous decoding function.
 * See other variants for asynchronous decoding: {@link decodeAsync}, {@link decodeStream}, or {@link decodeArrayStream}.
 *
 * @throws {@link RangeError} if the buffer is incomplete, including the case where the buffer is empty.
 * @throws {@link DecodeError} if the buffer contains invalid data.
 */
export declare function decode<ContextType = undefined>(buffer: ArrayLike<number> | ArrayBufferView | ArrayBufferLike, options?: DecoderOptions<SplitUndefined<ContextType>>): unknown;
/**
 * It decodes multiple MessagePack objects in a buffer.
 * This is corresponding to {@link decodeMultiStream}.
 *
 * @throws {@link RangeError} if the buffer is incomplete, including the case where the buffer is empty.
 * @throws {@link DecodeError} if the buffer contains invalid data.
 */
export declare function decodeMulti<ContextType = undefined>(buffer: ArrayLike<number> | BufferSource, options?: DecoderOptions<SplitUndefined<ContextType>>): Generator<unknown, void, unknown>;
