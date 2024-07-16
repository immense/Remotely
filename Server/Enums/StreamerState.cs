namespace Immense.RemoteControl.Server.Enums;

[Flags]
public enum StreamerState
{
    Unknown = 1 << 0,
    Connected = 1 << 1,
    Disconnected = 1 << 2,
    DisconnectExpected = 1 << 3,
    Reconnecting = 1 << 4,
    ChangingSessions = 1 << 5,
    WindowsLoggingOff = 1 << 6,
    WindowsShuttingDown = 1 << 7,
}
