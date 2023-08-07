using Remotely.Server.Enums;

namespace Remotely.Server.Models.Messages;

public record DeviceCardStateChangedMessage(string DeviceId, DeviceCardState State);