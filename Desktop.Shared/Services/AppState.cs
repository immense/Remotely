using Remotely.Desktop.Shared.Enums;
using Remotely.Desktop.Shared.Messages;
using Remotely.Shared.Models;
using Microsoft.Extensions.Logging;
using Bitbound.SimpleMessenger;
using System.Collections.Concurrent;

namespace Remotely.Desktop.Shared.Services;

public interface IAppState
{
    event EventHandler<ScreenCastRequest> ScreenCastRequested;

    event EventHandler<IViewer> ViewerAdded;

    event EventHandler<string> ViewerRemoved;
    string AccessKey { get; }
    Dictionary<string, string> ArgDict { get; }
    string Host { get; set; }
    bool IsElevate { get; }
    bool IsRelaunch { get; }
    AppMode Mode { get; set; }
    string OrganizationId { get; set; }
    string OrganizationName { get; }
    string PipeName { get; }
    string[] RelaunchViewers { get; }
    string RequesterName { get; }
    string SessionId { get; }
    ConcurrentDictionary<string, IViewer> Viewers { get; }

    void Configure(
        string host,
        AppMode mode,
        string sessionId,
        string accessKey,
        string requesterName,
        string organizationName,
        string pipeName,
        bool relaunch,
        string viewers,
        bool elevate);

    void InvokeScreenCastRequested(ScreenCastRequest viewerIdAndRequesterName);
    void InvokeViewerAdded(IViewer viewer);
    void InvokeViewerRemoved(string viewerID);
    void UpdateHost(string host);
}

public class AppState : IAppState
{
    private readonly Dictionary<string, string> _argDict = new();
    private readonly ILogger<AppState> _logger;
    private readonly IMessenger _messenger;
    private string _host = string.Empty;

    private bool _isConfigured;

    public AppState(IMessenger messenger, ILogger<AppState> logger)
    {
        _messenger = messenger;
        _logger = logger;
    }

    public event EventHandler<ScreenCastRequest>? ScreenCastRequested;

    public event EventHandler<IViewer>? ViewerAdded;

    public event EventHandler<string>? ViewerRemoved;

    public string AccessKey { get; private set; } = string.Empty;

    public Dictionary<string, string> ArgDict
    {
        get
        {
            if (!_argDict.Any())
            {
                ProcessArgs();
            }
            return _argDict;
        }
    }

    public string Host
    {
        get => _host;
        set
        {
            _host = value?.Trim()?.TrimEnd('/') ?? string.Empty;
            _messenger.Send(new AppStateHostChangedMessage(_host));
        }
    }

    public bool IsElevate { get; private set; }
    public bool IsRelaunch { get; private set; }
    public AppMode Mode { get; set; }
    public string OrganizationId { get; set; } = string.Empty;
    public string OrganizationName { get; private set; } = string.Empty;

    public string PipeName { get; private set; } = string.Empty;
    public string[] RelaunchViewers { get; private set; } = Array.Empty<string>();
    public string RequesterName { get; private set; } = string.Empty;
    public string SessionId { get; private set; } = string.Empty;
    public ConcurrentDictionary<string, IViewer> Viewers { get; } = new();
    public void Configure(
        string host,
        AppMode mode,
        string sessionId,
        string accessKey,
        string requesterName,
        string organizationName,
        string pipeName,
        bool relaunch,
        string viewers,
        bool elevate)
    {
        if (_isConfigured)
        {
            throw new InvalidOperationException("AppState has already been configured.");
        }

        _isConfigured = true;
        Host = host;
        Mode = mode;
        SessionId = sessionId;
        AccessKey = accessKey;
        RequesterName = requesterName;
        OrganizationName = organizationName;
        PipeName = pipeName;
        IsRelaunch = relaunch;
        RelaunchViewers = viewers.Split(",");
        IsElevate = elevate;
    }

    public void InvokeScreenCastRequested(ScreenCastRequest viewerIdAndRequesterName)
    {
        ScreenCastRequested?.Invoke(null, viewerIdAndRequesterName);
    }

    public void InvokeViewerAdded(IViewer viewer)
    {
        ViewerAdded?.Invoke(null, viewer);
    }

    public void InvokeViewerRemoved(string viewerID)
    {
        ViewerRemoved?.Invoke(null, viewerID);
    }

    public void UpdateHost(string host)
    {
        Host = host;
    }

    private void ProcessArgs()
    {
        var cmdArgs = Environment.GetCommandLineArgs();
        var args = Environment.GetCommandLineArgs()
            .SkipWhile(x => !x.StartsWith("-"))
            .ToArray();

        for (var i = 0; i < args.Length; i += 2)
        {
            try
            {
                var key = args[i];
                if (key != null)
                {
                    if (!key.Contains('-'))
                    {
                        _logger.LogWarning("Command line arguments are invalid.  Key: {key}", key);
                        i -= 1;
                        continue;
                    }

                    key = key.Trim().TrimStart('-').TrimStart('-').ToLower();

                    _argDict.Add(key, args[i + 1].Trim());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing args.");
            }

        }
    }
}
