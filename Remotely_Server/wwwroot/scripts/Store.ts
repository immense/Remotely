export const Store = new class {
    InputHistoryPosition = -1;
    CommandCompletionPosition = -1;
    InputHistoryItems: Array<string> = [];
    IsDisconnectExpected: boolean;
}