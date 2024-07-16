using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Server.Enums;
using Immense.RemoteControl.Server.Models;
using Immense.RemoteControl.Server.Services;
using Immense.RemoteControl.Shared.Enums;
using Immense.RemoteControl.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Immense.RemoteControl.Server.Hubs;

public class DesktopHub : Hub<IDesktopHubClient>
{
    private readonly IHubEventHandler _hubEvents;
    private readonly ILogger<DesktopHub> _logger;
    private readonly IRemoteControlSessionCache _sessionCache;
    private readonly IDesktopStreamCache _streamCache;
    private readonly IHubContext<ViewerHub, IViewerHubClient> _viewerHub;

    public DesktopHub(
        IRemoteControlSessionCache sessionCache,
        IDesktopStreamCache streamCache,
        IHubContext<ViewerHub, IViewerHubClient> viewerHubContext,
        IHubEventHandler hubEvents,
        ILogger<DesktopHub> logger)
    {
        _sessionCache = sessionCache;
        _streamCache = streamCache;
        _viewerHub = viewerHubContext;
        _hubEvents = hubEvents;
        _logger = logger;
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

    private HashSet<string> ViewerList => SessionInfo.ViewerList;

    public async Task DisconnectViewer(string viewerID, bool notifyViewer)
    {
        ViewerList.Remove(viewerID);

        if (notifyViewer)
        {
            await _viewerHub.Clients.Client(viewerID).ViewerRemoved();
        }
    }

    public async Task<string> GetSessionID()
    {
        using var scope = _logger.BeginScope(nameof(GetSessionID));

        SessionInfo.Mode = RemoteControlMode.Attended;

        var random = new Random();
        var sessionId = string.Empty;

        while (true)
        {
            sessionId = "";
            for (var i = 0; i < 3; i++)
            {
                sessionId += random.Next(0, 999).ToString().PadLeft(3, '0');
            }

            SessionInfo.AttendedSessionId = sessionId;
            if (_sessionCache.TryAdd(sessionId, SessionInfo))
            {
                break;
            }
            await Task.Yield();
        }

        SessionInfo.SetSessionReadyState(true);
        return sessionId;
    }

    public Task NotifyRequesterUnattendedReady()
    {
        using var scope = _logger.BeginScope(nameof(NotifyRequesterUnattendedReady));

        if (!_sessionCache.TryGetValue($"{SessionInfo.UnattendedSessionId}", out var session))
        {
            _logger.LogError("Connection not found in cache.");
            return Task.CompletedTask;
        }

        session.SetSessionReadyState(true);
        return Task.CompletedTask;
    }

    public async Task NotifySessionChanged(SessionSwitchReasonEx reason, int currentSessionId)
    {
        SessionInfo.StreamerState = StreamerState.ChangingSessions | StreamerState.DisconnectExpected;
        await _viewerHub.Clients.Clients(ViewerList).ShowMessage("Changing sessions");
        await _hubEvents.NotifySessionChanged(SessionInfo, reason, currentSessionId);
    }

    public async Task NotifySessionEnding(SessionEndReasonsEx reason)
    {
        switch (reason)
        {
            case SessionEndReasonsEx.Logoff:
                SessionInfo.StreamerState = StreamerState.WindowsLoggingOff | StreamerState.DisconnectExpected;
                await _viewerHub.Clients.Clients(ViewerList).ShowMessage("Windows session ending");
                await _hubEvents.NotifySessionChanged(SessionInfo, SessionSwitchReasonEx.SessionLogoff, -1);
                break;
            case SessionEndReasonsEx.SystemShutdown:
                SessionInfo.StreamerState = StreamerState.WindowsShuttingDown | StreamerState.DisconnectExpected;
                await _viewerHub.Clients.Clients(ViewerList).ShowMessage("Waiting for device to restart");
                break;
            default:
                break;
        }
    }

    public Task NotifyViewersRelaunchedScreenCasterReady(string[] viewerIDs)
    {
        SessionInfo.DesktopConnectionId = Context.ConnectionId;
        return _viewerHub.Clients
            .Clients(viewerIDs)
            .RelaunchedScreenCasterReady(
                SessionInfo.UnattendedSessionId,
                SessionInfo.AccessKey);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogDebug("Desktop app disconnected. Streamer State: {state}.  Viewer Count: {count}",
            SessionInfo.StreamerState,
            ViewerList.Count);

        SessionInfo.SetSessionReadyState(false);

        if (SessionInfo.Mode == RemoteControlMode.Attended)
        {
            SessionInfo.StreamerState = StreamerState.Disconnected;
            _ = _sessionCache.TryRemove(SessionInfo.AttendedSessionId, out _);
            await _viewerHub.Clients.Clients(ViewerList).ScreenCasterDisconnected();
        }
        else if (
            SessionInfo.Mode == RemoteControlMode.Unattended &&
            !SessionInfo.StreamerState.HasFlag(StreamerState.DisconnectExpected))
        {
            if (ViewerList.Count > 0)
            {
                SessionInfo.StreamerState = StreamerState.Reconnecting;
                await _viewerHub.Clients.Clients(ViewerList).Reconnecting();
                await _hubEvents.RestartScreenCaster(SessionInfo);
            }
            else
            {
                _ = _sessionCache.TryRemove($"{SessionInfo.UnattendedSessionId}", out _);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task<Result<string>> PingViewer(string viewerConnectionId)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var response = await _viewerHub.Clients.Client(viewerConnectionId).PingViewer(cts.Token);
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to ping viewer with connection ID {connectionId}.", viewerConnectionId);
            return Result.Fail<string>("Failed to ping viewer.");
        }
    }

    public Task ReceiveAttendedSessionInfo(string machineName)
    {
        SessionInfo.DesktopConnectionId = Context.ConnectionId;
        SessionInfo.StartTime = DateTimeOffset.Now;
        SessionInfo.MachineName = machineName;

        return Task.CompletedTask;
    }

    public Task<Result> ReceiveUnattendedSessionInfo(Guid unattendedSessionId, string accessKey, string machineName, string requesterName, string organizationName)
    {
        if (_sessionCache.TryGetValue($"{unattendedSessionId}", out var existingSession) &&
            !string.IsNullOrWhiteSpace(existingSession.AccessKey) &&
            accessKey != existingSession.AccessKey)
        {
            _logger.LogWarning(
                "A desktop session tried to take over an existing session, " +
                "but the access key didn't match.");
            var result = Result.Fail("SessionId already exists on the server.");
            return Task.FromResult(result);
        }

        SessionInfo = _sessionCache.GetOrAdd($"{unattendedSessionId}", (key) => SessionInfo);

        SessionInfo.Mode = RemoteControlMode.Unattended;
        SessionInfo.DesktopConnectionId = Context.ConnectionId;
        SessionInfo.StartTime = DateTimeOffset.Now;
        SessionInfo.UnattendedSessionId = unattendedSessionId;
        SessionInfo.AccessKey = accessKey;
        SessionInfo.MachineName = machineName;
        SessionInfo.RequesterName = requesterName;
        SessionInfo.OrganizationName = organizationName;

        return Task.FromResult(Result.Ok());
    }
    public Task SendConnectionFailedToViewers(List<string> viewerIDs)
    {
        return _viewerHub.Clients.Clients(viewerIDs).ConnectionFailed();
    }

    public Task SendConnectionRequestDenied(string viewerID)
    {
        return _viewerHub.Clients.Client(viewerID).ConnectionRequestDenied();
    }

    public async Task SendDesktopStream(IAsyncEnumerable<byte[]> stream, Guid streamId)
    {
        using var signaler = _streamCache.GetOrAdd(streamId, key => new StreamSignaler(streamId));
        signaler.DesktopConnectionId = Context.ConnectionId;

        try
        {
            signaler.Stream = stream;
            signaler.ReadySignal.Release();

            await _hubEvents.NotifyRemoteControlStarted(SessionInfo);
            // TODO: We can remove the timeout once we implement add a
            // timeout for viewer idle (i.e. no input).
            await signaler.EndSignal.WaitAsync(TimeSpan.FromHours(8));
            await _hubEvents.NotifyRemoteControlEnded(SessionInfo);
        }
        finally
        {
            _ = _streamCache.TryRemove(signaler.StreamId, out _);
        }
    }

    public Task SendDtoToViewer(byte[] dto, string viewerId)
    {
        return _viewerHub.Clients.Client(viewerId).SendDtoToViewer(dto);
    }

    public Task SendMessageToViewer(string viewerId, string message)
    {
        return _viewerHub.Clients.Client(viewerId).ShowMessage(message);
    }
}
