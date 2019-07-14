export const Store = new class {
    CommandCompletionPosition = -1;
    CommandCompletionTimeout: number;
    InputHistoryPosition = -1;
    InputHistoryItems: Array<string> = [];
    IsDisconnectExpected: boolean;
}