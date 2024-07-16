export enum HubConnectionState {
    /** The hub connection is disconnected. */
    Disconnected = "Disconnected",
    /** The hub connection is connecting. */
    Connecting = "Connecting",
    /** The hub connection is connected. */
    Connected = "Connected",
    /** The hub connection is disconnecting. */
    Disconnecting = "Disconnecting",
    /** The hub connection is reconnecting. */
    Reconnecting = "Reconnecting",
}