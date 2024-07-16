using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Server.Enums;
using Immense.RemoteControl.Server.Filters;
using Immense.RemoteControl.Server.Models;
using Immense.RemoteControl.Server.Services;
using Immense.RemoteControl.Shared;
using Immense.RemoteControl.Shared.Interfaces;
using Immense.RemoteControl.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Immense.RemoteControl.Server.Hubs;

[ServiceFilter(typeof(ViewerAuthorizationFilter))]
public class ViewerHub : Hub<IViewerHubClient>
{
    private readonly IHubContext<DesktopHub, IDesktopHubClient> _desktopHub;
    private readonly IViewerOptionsProvider _viewerOptionsProvider;
    private readonly ISessionRecordingSink _sessionRecordingSink;
    private readonly IRemoteControlSessionCache _desktopSessionCache;
    private readonly IHubEventHandler _hubEvents;
    private readonly ILogger<ViewerHub> _logger;
    private readonly IDesktopStreamCache _streamCache;

    public ViewerHub(
        IHubEventHandler hubEvents,
        IRemoteControlSessionCache desktopSessionCache,
        IDesktopStreamCache streamCache,
        IHubContext<DesktopHub, IDesktopHubClient> desktopHub,
        IViewerOptionsProvider viewerOptionsProvider,
        ISessionRecordingSink sessionRecordingSink,
        ILogger<ViewerHub> logger)
    {
        _hubEvents = hubEvents;
        _desktopSessionCache = desktopSessionCache;
        _streamCache = streamCache;
        _desktopHub = desktopHub;
        _viewerOptionsProvider = viewerOptionsProvider;
        _sessionRecordingSink = sessionRecordingSink;
        _logger = logger;
    }

    private string RequesterDisplayName
    {
        get
        {
            if (Context.Items.TryGetValue(nameof(RequesterDisplayName), out var result) &&
                result is string requesterName)
            {
                return requesterName;
            }
            return string.Empty;
        }
        set
        {
            Context.Items[nameof(RequesterDisplayName)] = value;
        }
    }

    private RemoteControlSession SessionInfo
    {
        get
        {
            if (Context.Items.TryGetValue(nameof(SessionInfo), out var result) &&
                result is RemoteControlSession session)
            {
                return session;
            }

            var newSession = new RemoteControlSession();
            Context.Items[nameof(SessionInfo)] = newSession;
            return newSession;
        }
        set
        {
            Context.Items[nameof(SessionInfo)] = value;
        }
    }
    public async Task<Result> ChangeWindowsSession(int targetWindowsSession)
    {
        if (SessionInfo.Mode != RemoteControlMode.Unattended)
        {
            return Result.Fail("Only available in unattended mode.");
        }

        SessionInfo.ViewerList.Remove(Context.ConnectionId);
        await _desktopHub.Clients
            .Client(SessionInfo.DesktopConnectionId)
            .ViewerDisconnected(Context.ConnectionId);

        SessionInfo = SessionInfo.CreateNew();
        _desktopSessionCache.AddOrUpdate($"{SessionInfo.UnattendedSessionId}", SessionInfo);

        await _hubEvents.ChangeWindowsSession(SessionInfo, Context.ConnectionId, targetWindowsSession);
        return Result.Ok();
    }

    public async IAsyncEnumerable<byte[]> GetDesktopStream()
    {
        var sessionResult = await _streamCache.WaitForStreamSession(
            SessionInfo.StreamId,
            Context.ConnectionId,
            TimeSpan.FromSeconds(30));

        if (!sessionResult.IsSuccess)
        {
            _logger.LogError("Timed out while waiting for desktop stream.");
            await Clients.Caller.ShowMessage("Request timed out");
            yield break;
        }

        var signaler = sessionResult.Value;

        if (signaler.Stream is null)
        {
            _logger.LogError("Stream was null.");
            yield break;
        }

        SessionInfo.StreamerState = StreamerState.Connected;

        try
        {
            await foreach (var chunk in signaler.Stream)
            {
                yield return chunk;
            }
        }
        finally
        {
            signaler.EndSignal.Release();
            _logger.LogInformation("Streaming session ended for {sessionId}.", SessionInfo.StreamId);
        }
    }

    public async Task<RemoteControlViewerOptions> GetViewerOptions()
    {
        return await _viewerOptionsProvider.GetViewerOptions();
    }

    public Task InvokeCtrlAltDel()
    {
        return _hubEvents.InvokeCtrlAltDel(SessionInfo, Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (!string.IsNullOrWhiteSpace(SessionInfo.DesktopConnectionId))
        {
            await _desktopHub.Clients
                .Client(SessionInfo.DesktopConnectionId)
                .ViewerDisconnected(Context.ConnectionId);
        }

        SessionInfo.ViewerList.Remove(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public Task SendDtoToClient(byte[] dtoWrapper)
    {
        if (string.IsNullOrWhiteSpace(SessionInfo.DesktopConnectionId))
        {
            return Task.CompletedTask;
        }

        return _desktopHub.Clients
            .Client(SessionInfo.DesktopConnectionId)
            .SendDtoToClient(dtoWrapper, Context.ConnectionId);
    }
    public async Task<Result> SendScreenCastRequestToDevice(string sessionId, string accessKey, string requesterName)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Result.Fail("Session ID cannot be empty.");
        }

        if (!_desktopSessionCache.TryGetValue(sessionId, out var session))
        {
            return Result.Fail("Session ID not found.");
        }

        if (session.Mode == RemoteControlMode.Unattended &&
            accessKey != session.AccessKey)
        {
            _logger.LogError("Access key does not match for unattended session.  " +
                "Session ID: {sessionId}.  " +
                "Requester Name: {requesterName}.  " +
                "Requester Connection ID: {connectionId}",
                sessionId,
                requesterName,
                Context.ConnectionId);
            return Result.Fail("Authorization failed.");
        }

        SessionInfo = session;
        SessionInfo.ViewerList.Add(Context.ConnectionId);
        SessionInfo.StreamId = Guid.NewGuid();
        RequesterDisplayName = requesterName;

        if (Context.User?.Identity?.IsAuthenticated == true)
        {
            SessionInfo.RequesterUserName = Context.User.Identity.Name ?? string.Empty;
        }

        var logMessage = $"Remote control session requested.  " +
                            $"Login ID (if logged in): {Context.User?.Identity?.Name}.  " +
                            $"Machine Name: {SessionInfo.MachineName}.  " +
                            $"Stream ID: {SessionInfo.StreamId}.  " +
                            $"Requester Name (if specified): {RequesterDisplayName}.  " +
                            $"Connection ID: {Context.ConnectionId}. User ID: {Context.UserIdentifier}.  " +
                            $"Screen Caster Connection ID: {SessionInfo.DesktopConnectionId}.  " +
                            $"Mode: {SessionInfo.Mode}.  " +
                            $"Requester IP Address: {Context.GetHttpContext()?.Connection?.RemoteIpAddress}";

        _logger.LogInformation("{msg}", logMessage);

        if (SessionInfo.Mode == RemoteControlMode.Unattended)
        {
            if (SessionInfo.RequireConsent)
            {
                var request = new RemoteControlAccessRequest(
                    Context.ConnectionId,
                    RequesterDisplayName,
                    SessionInfo.OrganizationName);

                var result = await _desktopHub.Clients
                    .Client(SessionInfo.DesktopConnectionId)
                    .PromptForAccess(request);

                if (result != Shared.Enums.PromptForAccessResult.Accepted)
                {
                    return Result.Fail($"Access request failed.  Reason: {result}");
                }
            }

            SessionInfo.RequireConsent = false;

            await _desktopHub.Clients
                .Client(SessionInfo.DesktopConnectionId)
                .GetScreenCast(
                    Context.ConnectionId,
                    RequesterDisplayName,
                    SessionInfo.NotifyUserOnStart,
                    SessionInfo.StreamId);
        }
        else
        {
            SessionInfo.Mode = RemoteControlMode.Attended;
            await _desktopHub.Clients
                .Client(SessionInfo.DesktopConnectionId)
                .RequestScreenCast(
                    Context.ConnectionId,
                    RequesterDisplayName,
                    SessionInfo.NotifyUserOnStart,
                    SessionInfo.StreamId);
        }

        return Result.Ok();
    }

    public async Task StoreSessionRecording(IAsyncEnumerable<byte[]> webmStream)
    {
        try
        {
            await _sessionRecordingSink.SinkWebmStream(webmStream, SessionInfo);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Session recording stopped for stream {streamId}.", SessionInfo.StreamId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while storing session recording for stream {streamId}.", SessionInfo.StreamId);
        }
    }
}
