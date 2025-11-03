import { ExtData } from "./ExtData";
export type ExtensionDecoderType<ContextType> = (data: Uint8Array, extensionType: number, context: ContextType) => unknown;
export type ExtensionEncoderType<ContextType> = (input: unknown, context: ContextType) => Uint8Array | ((dataPos: number) => Uint8Array) | null;
export type ExtensionCodecType<ContextType> = {
    __brand?: ContextType;
    tryToEncode(object: unknown, context: ContextType): ExtData | null;
    decode(data: Uint8Array, extType: number, context: ContextType): unknown;
};
export declare class ExtensionCodec<ContextType = undefined> implements ExtensionCodecType<ContextType> {
    static readonly defaultCodec: ExtensionCodecType<undefined>;
    __brand?: ContextType;
    private readonly builtInEncoders;
    private readonly builtInDecoders;
    private readonly encoders;
    private readonly decoders;
    constructor();
    register({ type, encode, decode, }: {
        type: number;
        encode: ExtensionEncoderType<ContextType>;
        decode: ExtensionDecoderType<ContextType>;
    }): void;
    tryToEncode(object: unknown, context: ContextType): ExtData | null;
    decode(data: Uint8Array, type: number, context: ContextType): unknown;
}
