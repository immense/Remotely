namespace Remotely.Server.Models.Messages;

public record ChatReceivedMessage(string DeviceId, string DeviceName, string MessageText, bool DidDisconnect = false);