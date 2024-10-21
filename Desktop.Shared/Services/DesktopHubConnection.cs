using Remotely.Desktop.Shared.Abstractions;
using Remotely.Desktop.Shared.Messages;
using Remotely.Shared.Enums;
using Remotely.Shared.Interfaces;
using Remotely.Shared.Models;
using Bitbound.SimpleMessenger;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Primitives;
using System.Diagnostics;
using Remotely.Desktop.Native.Windows;

namespace Remotely.Desktop.Shared.Services;

public interface IDesktopHubConnection
{
    HubConnection? Connection { get; }
    HubConnectionState ConnectionState { get; }
    bool IsConnected { get; }

    Task<Result<TimeSpan>> CheckRoundtripLatency(string viewerConnectionId);
    Task<bool> Connect(TimeSpan timeout, CancellationToken cancellationToken);
    Task Disconnect();
    Task DisconnectAllViewers();
    Task DisconnectViewer(IViewer viewer, bool notifyViewer);
    Task<string> GetSessionID();
    Task NotifyRequesterUnattendedReady();
    Task NotifyViewersRelaunchedScreenCasterReady(string[] viewerIDs);
    Task SendAttendedSessionInfo(string machineName);

    Task SendConnectionFailedToViewers(List<string> viewerIDs);
    Task SendConnectionRequestDenied(string viewerID);
    Task SendDtoToViewer<T>(T dto, string viewerId);

    Task SendMessageToViewer(string viewerID, string message);
    Task<Result> SendUnattendedSessionInfo(string sessionId, string accessKey, string machineName, string requesterName, string organizationName);
}

public class DesktopHubConnection : IDesktopHubConnection, IDesktopHubClient
{
    private readonly IAppState _appState;

    private readonly ILogger<DesktopHubConnection> _logger;
    private readonly IDtoMessageHandler _messageHandler;
    private readonly IRemoteControlAccessService _remoteControlAccessService;
    private readonly IServiceProvider _serviceProvider;

    public DesktopHubConnection(
        IDtoMessageHandler messageHandler,
        IServiceProvider serviceProvider,
        IAppState appState,
        IRemoteControlAccessService remoteControlAccessService,
        IMessenger messenger,
        ILogger<DesktopHubConnection> logger)
    {
        _messageHandler = messageHandler;
        _remoteControlAccessService = remoteControlAccessService;
        _serviceProvider = serviceProvider;
        _appState = appState;
        _logger = logger;

        messenger.Register<WindowsSessionEndingMessage>(this, HandleWindowsSessionEnding);
        messenger.Register<WindowsSessionSwitchedMessage>(this, HandleWindowsSessionChanged);
    }

    public HubConnection? Connection { get; private set; }
    public HubConnectionState ConnectionState => Connection?.State ?? HubConnectionState.Disconnected;
    public bool IsConnected => Connection?.State == HubConnectionState.Connected;

    public async Task<Result<TimeSpan>> CheckRoundtripLatency(string viewerConnectionId)
    {
        try
        {
            if (Connection is null)
            {
                return Result.Fail<TimeSpan>("Connection is not yet established.");
            }
            var sw = Stopwatch.StartNew();
            var result = await Connection.InvokeAsync<Result<string>>("PingViewer", viewerConnectionId);
            if (result.IsSuccess)
            {
                return Result.Ok(sw.Elapsed);
            }
            return Result.Fail<TimeSpan>("Latency check failed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check latency.");
            return Result.Fail<TimeSpan>("An error occurred while checking latency.");
        }
    }

    public async Task<bool> Connect(TimeSpan timeout, CancellationToken cancellationToken)
    {
        try
        {
            if (Connection is not null)
            {
                await Connection.DisposeAsync();
            }

            var result = BuildConnection();
            if (!result.IsSuccess)
            {
                return false;
            }

            Connection = result.Value;

            ApplyConnectionHandlers(Connection);

            var sw = Stopwatch.StartNew();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Connecting to server.");

                    await Connection.StartAsync(cancellationToken);

                    _logger.LogInformation("Connected to server.");

                    break;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning("Failed to connect to server.  Status Code: {code}", ex.StatusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in hub connection.");
                }
                await Task.Delay(3_000, cancellationToken);

                if (sw.Elapsed > timeout)
                {
                    _logger.LogWarning("Timed out while trying to connect to desktop hub.");
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while connecting to hub.");
            return false;
        }
    }

    public async Task Disconnect()
    {
        try
        {
            if (Connection is not null)
            {
                await Connection.StopAsync();
                await Connection.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disconnecting websocket.");
        }
    }

    public async Task Disconnect(string reason)
    {
        _logger.LogInformation("Disconnecting caster socket.  Reason: {reason}", reason);
        await DisconnectAllViewers();
    }

    public async Task DisconnectAllViewers()
    {
        foreach (var viewer in _appState.Viewers.Values.ToList())
        {
            await DisconnectViewer(viewer, true);
        }
    }

    public Task DisconnectViewer(IViewer viewer, bool notifyViewer)
    {
        if (Connection is null)
        {
            return Task.CompletedTask;
        }

        viewer.DisconnectRequested = true;
        viewer.Dispose();
        return Connection.SendAsync("DisconnectViewer", viewer.ViewerConnectionId, notifyViewer);
    }

    public Task GetScreenCast(
        string viewerId,
        string requesterName,
        bool notifyUser,
        Guid streamId)
    {
        // We don't want to tie up the invocation from the server, so we'll
        // start this in a new task.
        _ = Task.Run(async () =>
        {
            try
            {
                using var screenCaster = _serviceProvider.GetRequiredService<IScreenCaster>();
                await screenCaster.BeginScreenCasting(
                    new ScreenCastRequest()
                    {
                        NotifyUser = notifyUser,
                        ViewerId = viewerId,
                        RequesterName = requesterName,
                        StreamId = streamId
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while casting screen.");
            }
        });

        return Task.CompletedTask;
    }

    public async Task<string> GetSessionID()
    {
        if (Connection is null)
        {
            return string.Empty;
        }

        return await Connection.InvokeAsync<string>("GetSessionID");
    }

    public Task NotifyRequesterUnattendedReady()
    {
        if (Connection is null)
        {
            return Task.CompletedTask;
        }

        return Connection.SendAsync("NotifyRequesterUnattendedReady");
    }


    public Task NotifyViewersRelaunchedScreenCasterReady(string[] viewerIDs)
    {
        if (Connection is null)
        {
            return Task.CompletedTask;
        }

        return Connection.SendAsync("NotifyViewersRelaunchedScreenCasterReady", viewerIDs);
    }

    public async Task<PromptForAccessResult> PromptForAccess(RemoteControlAccessRequest accessRequest)
    {
        try
        {
            // TODO: Add this to Win32Interop service/interface when it's
            // extracted from current static class.
            if (OperatingSystem.IsWindows() &&
                Shlwapi.IsOS(OsType.OS_ANYSERVER) &&
                Process.GetCurrentProcess().SessionId == Kernel32.WTSGetActiveConsoleSessionId())
            {
                // Bypass "consent prompt" if we're targeting the console session
                // on a Windows Server OS.
                return PromptForAccessResult.Accepted;
            }
            await SendMessageToViewer(accessRequest.ViewerConnectionId, "Asking user for permission");
            return await _remoteControlAccessService.PromptForAccess(
                accessRequest.RequesterDisplayName,
                accessRequest.OrganizationName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while applying connection handlers.");
            return PromptForAccessResult.Error;
        }
    }

    public Task RequestScreenCast(string viewerId, string requesterName, bool notifyUser, Guid streamId)
    {
        _appState.InvokeScreenCastRequested(new ScreenCastRequest()
        {
            NotifyUser = notifyUser,
            ViewerId = viewerId,
            RequesterName = requesterName,
            StreamId = streamId
        });
        return Task.CompletedTask;
    }

    public Task SendAttendedSessionInfo(string machineName)
    {
        if (Connection is null)
        {
            return Task.CompletedTask;
        }

        return Connection.InvokeAsync("ReceiveAttendedSessionInfo", machineName);
    }

    public Task SendConnectionFailedToViewers(List<string> viewerIDs)
    {
        if (Connection is null)
        {
            return Task.CompletedTask;
        }

        return Connection.SendAsync("SendConnectionFailedToViewers", viewerIDs);
    }

    public Task SendConnectionRequestDenied(string viewerID)
    {
        if (Connection is null)
        {
            return Task.CompletedTask;
        }
        return Connection.SendAsync("SendConnectionRequestDenied", viewerID);
    }

    public async Task SendDtoToClient(byte[] dtoWrapper, string viewerConnectionId)
    {
        if (_appState.Viewers.TryGetValue(viewerConnectionId, out var viewer))
        {
            await _messageHandler.ParseMessage(viewer, dtoWrapper);
        }
    }

    public Task SendDtoToViewer<T>(T dto, string viewerId)
    {
        if (Connection is null)
        {
            return Task.CompletedTask;
        }

        var serializedDto = MessagePack.MessagePackSerializer.Serialize(dto);
        return Connection.SendAsync("SendDtoToViewer", serializedDto, viewerId);
    }

    public Task SendMessageToViewer(string viewerID, string message)
    {
        if (Connection is null)
        {
            return Task.CompletedTask;
        }

        return Connection.SendAsync("SendMessageToViewer", viewerID, message);
    }

    public async Task<Result> SendUnattendedSessionInfo(string unattendedSessionId, string accessKey, string machineName, string requesterName, string organizationName)
    {
        if (Connection is null)
        {
            return Result.Fail("Connection hasn't been made yet.");
        }

        return await Connection.InvokeAsync<Result>("ReceiveUnattendedSessionInfo", unattendedSessionId, accessKey, machineName, requesterName, organizationName);
    }

    public async Task ViewerDisconnected(string viewerId)
    {
        if (Connection is null)
        {
            return;
        }

        await Connection.SendAsync("DisconnectViewer", viewerId, false);
        if (_appState.Viewers.TryRemove(viewerId, out var viewer))
        {
            viewer.DisconnectRequested = true;
            viewer.Dispose();
        }
        _appState.InvokeViewerRemoved(viewerId);
    }

    private void ApplyConnectionHandlers(HubConnection connection)
    {
        connection.Closed += (ex) =>
        {
            _logger.LogWarning(ex, "Connection closed.");
            return Task.CompletedTask;
        };

        // TODO: Replace parameters with singular DTOs for both client and server methods.
        connection.On<string>(nameof(Disconnect), Disconnect);
        connection.On<string, string, bool, Guid>(nameof(GetScreenCast), GetScreenCast);
        connection.On<string, string, bool, Guid>(nameof(RequestScreenCast), RequestScreenCast);
        connection.On<byte[], string>(nameof(SendDtoToClient), SendDtoToClient);
        connection.On<string>(nameof(ViewerDisconnected), ViewerDisconnected);
        connection.On<RemoteControlAccessRequest, PromptForAccessResult>(nameof(PromptForAccess), PromptForAccess);
    }

    private Result<HubConnection> BuildConnection()
    {
        try
        {
            if (!Uri.TryCreate(_appState.Host, UriKind.Absolute, out _))
            {
                return Result.Fail<HubConnection>("Invalid server URI.");
            }

            var builder = _serviceProvider.GetRequiredService<IHubConnectionBuilder>();

            var connection = builder
                .WithUrl($"{_appState.Host.Trim().TrimEnd('/')}/hubs/desktop")
                .AddMessagePackProtocol()
                .WithAutomaticReconnect(new RetryPolicy())
                .Build();
            return Result.Ok(connection);
        }
        catch (Exception ex)
        {
            return Result.Fail<HubConnection>(ex);
        }
    }

    private async Task HandleWindowsSessionChanged(object subscriber, WindowsSessionSwitchedMessage message)
    {
        try
        {
            if (Connection is null)
            {
                return;
            }

            await Connection.SendAsync("NotifySessionChanged", message.Reason, message.SessionId);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while notifying of session change.");
        }
    }

    private async Task HandleWindowsSessionEnding(object subscriber, WindowsSessionEndingMessage message)
    {
        try
        {
            if (Connection is null)
            {
                return;
            }

            await Connection.SendAsync("NotifySessionEnding", message.Reason);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while notifying of session ending.");
        }
    }
    private class RetryPolicy : IRetryPolicy
    {
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(3);
        }
    }
}
