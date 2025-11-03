import type { DecoderOptions } from "./Decoder";
import type { ReadableStreamLike } from "./utils/stream";
import type { SplitUndefined } from "./context";
/**
 * @throws {@link RangeError} if the buffer is incomplete, including the case where the buffer is empty.
 * @throws {@link DecodeError} if the buffer contains invalid data.
 */
export declare function decodeAsync<ContextType = undefined>(streamLike: ReadableStreamLike<ArrayLike<number> | BufferSource>, options?: DecoderOptions<SplitUndefined<ContextType>>): Promise<unknown>;
/**
 * @throws {@link RangeError} if the buffer is incomplete, including the case where the buffer is empty.
 * @throws {@link DecodeError} if the buffer contains invalid data.
 */
export declare function decodeArrayStream<ContextType>(streamLike: ReadableStreamLike<ArrayLike<number> | BufferSource>, options?: DecoderOptions<SplitUndefined<ContextType>>): AsyncGenerator<unknown, void, unknown>;
/**
 * @throws {@link RangeError} if the buffer is incomplete, including the case where the buffer is empty.
 * @throws {@link DecodeError} if the buffer contains invalid data.
 */
export declare function decodeMultiStream<ContextType>(streamLike: ReadableStreamLike<ArrayLike<number> | BufferSource>, options?: DecoderOptions<SplitUndefined<ContextType>>): AsyncGenerator<unknown, void, unknown>;
