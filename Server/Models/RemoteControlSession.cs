using Immense.RemoteControl.Server.Enums;
using Immense.RemoteControl.Server.Services;
using Immense.RemoteControl.Shared.Helpers;

namespace Immense.RemoteControl.Server.Models;

/// <summary>
/// Contains data related to a remote control session.  Consuming projects
/// are expected to derive a class from this and add project-specific properties.
/// They should add these derived classes to<see cref="IRemoteControlSessionCache"/>
/// when sessions are created.
/// </summary>
public class RemoteControlSession : IDisposable
{
    private readonly ManualResetEventSlim _sessionReadySignal = new(false);
    private bool _disposedValue;
    private StreamerState _streamerState;

    public RemoteControlSession()
    {
        Created = DateTimeOffset.Now;
    }

    public string AccessKey { get; internal set; } = string.Empty;
    public string AgentConnectionId { get; set; } = string.Empty;
    public string AttendedSessionId { get; set; } = string.Empty;
    public DateTimeOffset Created { get; internal set; }
    public string DesktopConnectionId { get; internal set; } = string.Empty;
    public DateTimeOffset LastStateChange { get; private set; } = DateTimeOffset.Now;
    public string MachineName { get; internal set; } = string.Empty;
    public RemoteControlMode Mode { get; internal set; }
    /// <summary>
    /// Whether to notify the user via system notification when a remote
    /// control session has started.
    /// </summary>
    public bool NotifyUserOnStart { get; set; } = true;

    public string OrganizationName { get; internal set; } = string.Empty;
    public string RelativeAccessUri => $"/RemoteControl/Viewer?mode=Unattended&sessionId={UnattendedSessionId}&accessKey={AccessKey}&viewonly=False";
    public string RequesterName { get; set; } = string.Empty;
    public string RequesterUserName { get; internal set; } = string.Empty;
    /// <summary>
    /// Whether the remote control session requires the end user's consent
    /// before the view can connect.
    /// </summary>
    public bool RequireConsent { get; set; }

    public DateTimeOffset StartTime { get; internal set; }

    /// <summary>
    /// Current state of the streamer (desktop process) that's associated
    /// with this session.
    /// </summary>
    public StreamerState StreamerState
    {
        get => _streamerState;
        set
        {
            _streamerState = value;
            LastStateChange = DateTimeOffset.Now;
        }
    }

    public Guid StreamId { get; internal set; }
    public Guid UnattendedSessionId { get; set; }
    public string UserConnectionId { get; set; } = string.Empty;

    /// <summary>
    /// Contains a collection of viewer SignalR connection IDs.
    /// </summary>
    public HashSet<string> ViewerList { get; private set; } = new();

    public bool ViewOnly { get; set; }
    /// <summary>
    /// Creates a new session based off this existing one, but with
    /// a new UnattendedSessionId and AccessKey.
    /// </summary>
    /// <returns></returns>
    public RemoteControlSession CreateNew()
    {
        if (Mode != RemoteControlMode.Unattended)
        {
            throw new InvalidOperationException("Only available in unattended mode.");
        }

        var clone = (RemoteControlSession)MemberwiseClone();
        clone.Created = DateTimeOffset.Now;
        clone.UnattendedSessionId = Guid.NewGuid();
        clone.AccessKey = RandomGenerator.GenerateAccessKey();
        clone.ViewerList = new();
        clone.ViewOnly = false;
        return clone;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public Task<bool> WaitForSessionReady(TimeSpan waitTime)
    {
        return Task.Run(() => _sessionReadySignal.Wait(waitTime));
    }

    public Task<bool> WaitForSessionReady(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            try
            {
                _sessionReadySignal.Wait(cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        });
    }

    internal void SetSessionReadyState(bool isReady)
    {
        if (isReady)
        {
            _sessionReadySignal.Set();
        }
        else
        {
            _sessionReadySignal.Reset();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _sessionReadySignal.Dispose();
            }
            _disposedValue = true;
        }
    }
}
