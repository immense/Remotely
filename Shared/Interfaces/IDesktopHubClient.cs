using Remotely.Shared.Enums;
using Remotely.Shared.Models;

namespace Remotely.Shared.Interfaces;

public interface IDesktopHubClient
{
    Task Disconnect(string reason);

    Task GetScreenCast(
        string viewerId,
        string requesterName,
        bool notifyUser,
        Guid streamId);

    Task<PromptForAccessResult> PromptForAccess(RemoteControlAccessRequest accessRequest);
    //Task ReceiveButtonAction(Remotely.Desktop.Shared.Messages.ButtonActionMessage msg);
    Task RequestScreenCast(
        string viewerId,
        string requesterName,
        bool notifyUser,
        Guid streamId);

    Task SendDtoToClient(byte[] dtoWrapper, string viewerConnectionId);

    Task ViewerDisconnected(string viewerId);
}
