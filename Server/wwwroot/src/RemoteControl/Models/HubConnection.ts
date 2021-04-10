export type HubConnection = {
    start: () => Promise<any>;
    connectionStarted: boolean;
    closedCallbacks: any[];
    invoke: (...rest) => any;
    stop: () => any;
}
