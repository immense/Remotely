export type SplitTypes<T, U> = U extends T ? (Exclude<T, U> extends never ? T : Exclude<T, U>) : T;
export type SplitUndefined<T> = SplitTypes<T, undefined>;
export type ContextOf<ContextType> = ContextType extends undefined ? object : {
    /**
     * Custom user-defined data, read/writable
     */
    context: ContextType;
};
