namespace Immense.RemoteControl.Shared.Interfaces;

public interface IViewerHubClient
{
    Task ConnectionFailed();

    Task ConnectionRequestDenied();

    Task<string> PingViewer(CancellationToken cancellationToken);

    Task ReconnectFailed();

    Task Reconnecting();

    Task RelaunchedScreenCasterReady(Guid newSessionId, string newAccessKey);

    Task ScreenCasterDisconnected();

    Task SendDtoToViewer(byte[] dtoWrapper);
    Task ShowMessage(string message);

    Task ViewerRemoved();
}
