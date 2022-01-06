export type HubConnection = {
    start: () => Promise<any>;
    onclose: (callback: () => any) => any;
    connectionStarted: boolean;
    invoke: (...rest) => any;
    stop: () => any;
}
