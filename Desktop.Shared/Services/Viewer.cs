using System.Collections.Concurrent;
using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Shared.Models;
using Microsoft.Extensions.Logging;
using Immense.RemoteControl.Shared.Helpers;
using Immense.RemoteControl.Shared.Models.Dtos;
using Immense.RemoteControl.Desktop.Shared.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using Immense.RemoteControl.Shared.Services;
using Immense.RemoteControl.Desktop.Shared.Native.Windows;

namespace Immense.RemoteControl.Desktop.Shared.Services;

public interface IViewer : IDisposable
{
    IScreenCapturer Capturer { get; }
    double CurrentFps { get; }
    double CurrentMbps { get; }
    bool DisconnectRequested { get; set; }
    bool HasControl { get; set; }
    int ImageQuality { get; }
    bool IsResponsive { get; }
    string Name { get; set; }
    TimeSpan RoundTripLatency { get; }
    string ViewerConnectionId { get; set; }

    void AppendSentFrame(SentFrame sentFrame);
    Task ApplyAutoQuality();
    Task CalculateMetrics();
    void IncrementFpsCount();
    Task SendAudioSample(byte[] audioSample);
    Task SendClipboardText(string clipboardText);
    Task SendCursorChange(CursorInfo cursorInfo);
    Task SendDesktopStream(IAsyncEnumerable<byte[]> asyncEnumerable, Guid streamId);
    Task SendFile(FileUpload fileUpload, Action<double> progressUpdateCallback, CancellationToken cancelToken);
    Task SendScreenData(string selectedDisplay, IEnumerable<string> displayNames, int screenWidth, int screenHeight);
    Task SendScreenSize(int width, int height);
    Task SendSessionMetrics(SessionMetricsDto metrics);
    Task SendWindowsSessions();
    void SetLastFrameReceived(DateTimeOffset timestamp);
    Task<bool> WaitForViewer();
}

public class Viewer : IViewer
{
    public const int DefaultQuality = 80;

    private readonly IAudioCapturer _audioCapturer;
    private readonly IClipboardService _clipboardService;
    private readonly IDesktopHubConnection _desktopHubConnection;
    private readonly ConcurrentQueue<DateTimeOffset> _fpsQueue = new();
    private readonly ILogger<Viewer> _logger;
    private readonly ConcurrentQueue<SentFrame> _sentFrames = new();
    private readonly ISystemTime _systemTime;
    private bool _disconnectRequested;
    private volatile int _framesSentSinceLastReceipt;
    private DateTimeOffset _lastFrameReceived = DateTimeOffset.Now;
    private DateTimeOffset _lastFrameSent = DateTimeOffset.Now;
    private int _pingFailures;

    public Viewer(
        string requesterName,
        string viewerHubConnectionId,
        IDesktopHubConnection desktopHubConnection,
        IScreenCapturer screenCapturer,
        IClipboardService clipboardService,
        IAudioCapturer audioCapturer,
        ISystemTime systemTime,
        ILogger<Viewer> logger)
    {
        Name = requesterName;
        ViewerConnectionId = viewerHubConnectionId;
        Capturer = screenCapturer;
        _desktopHubConnection = desktopHubConnection;
        _clipboardService = clipboardService;
        _audioCapturer = audioCapturer;
        _systemTime = systemTime;
        _logger = logger;

        _clipboardService.ClipboardTextChanged += ClipboardService_ClipboardTextChanged;
        _audioCapturer.AudioSampleReady += AudioCapturer_AudioSampleReady;
    }

    public IScreenCapturer Capturer { get; }
    public double CurrentFps { get; private set; }
    public double CurrentMbps { get; private set; }
    public bool DisconnectRequested
    {
        get => _disconnectRequested;
        set
        {
            _disconnectRequested = value;
        }
    }
    public bool HasControl { get; set; } = true;
    public int ImageQuality { get; private set; } = DefaultQuality;
    public bool IsResponsive { get; private set; } = true;
    public string Name { get; set; } = string.Empty;
    public TimeSpan RoundTripLatency { get; private set; }

    public string ViewerConnectionId { get; set; } = string.Empty;
    public void AppendSentFrame(SentFrame sentFrame)
    {
        Interlocked.Increment(ref _framesSentSinceLastReceipt);
        _lastFrameSent = sentFrame.Timestamp;
        _sentFrames.Enqueue(sentFrame);
    }

    public Task ApplyAutoQuality()
    {
        if (ImageQuality < DefaultQuality)
        {
            ImageQuality = Math.Min(DefaultQuality, ImageQuality + 2);
        }
        return Task.CompletedTask;
    }

    public async Task CalculateMetrics()
    {
        if (_desktopHubConnection.Connection is null)
        {
            return;
        }

        CalculateMbps();
        CalculateFps();
        await CalculateLatency();
    }

    public void Dispose()
    {
        DisconnectRequested = true;
        Disposer.TryDisposeAll(Capturer);
        GC.SuppressFinalize(this);
    }

    public void IncrementFpsCount()
    {
        _fpsQueue.Enqueue(_systemTime.Now);
    }

    public async Task SendAudioSample(byte[] audioSample)
    {
        var dto = new AudioSampleDto(audioSample);
        await TrySendToViewer(dto, DtoType.AudioSample, ViewerConnectionId);
    }

    public async Task SendClipboardText(string clipboardText)
    {
        var dto = new ClipboardTextDto(clipboardText);
        await TrySendToViewer(dto, DtoType.ClipboardText, ViewerConnectionId);
    }

    public async Task SendCursorChange(CursorInfo cursorInfo)
    {
        if (cursorInfo is null)
        {
            return;
        }

        var dto = new CursorChangeDto(cursorInfo.ImageBytes, cursorInfo.HotSpot.X, cursorInfo.HotSpot.Y, cursorInfo.CssOverride);
        await TrySendToViewer(dto, DtoType.CursorChange, ViewerConnectionId);
    }

    public async Task SendDesktopStream(IAsyncEnumerable<byte[]> stream, Guid streamId)
    {
        if (_desktopHubConnection.Connection is not null)
        {
            await _desktopHubConnection.Connection.SendAsync("SendDesktopStream", stream, streamId);
        }
    }

    public async Task SendFile(
        FileUpload fileUpload,
        Action<double> progressUpdateCallback,
        CancellationToken cancelToken)
    {
        try
        {
            var messageId = Guid.NewGuid().ToString();
            var fileDto = new FileDto()
            {
                EndOfFile = false,
                FileName = fileUpload.DisplayName,
                MessageId = messageId,
                StartOfFile = true
            };

            await TrySendToViewer(fileDto, DtoType.File, ViewerConnectionId);

            using var fs = File.OpenRead(fileUpload.FilePath);
            using var br = new BinaryReader(fs);
            while (fs.Position < fs.Length)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                fileDto = new FileDto()
                {
                    Buffer = br.ReadBytes(40_000),
                    FileName = fileUpload.DisplayName,
                    MessageId = messageId
                };

                await TrySendToViewer(fileDto, DtoType.File, ViewerConnectionId);

                progressUpdateCallback((double)fs.Position / fs.Length);
            }

            fileDto = new FileDto()
            {
                EndOfFile = true,
                FileName = fileUpload.DisplayName,
                MessageId = messageId,
                StartOfFile = false
            };

            await TrySendToViewer(fileDto, DtoType.File, ViewerConnectionId);

            progressUpdateCallback(1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending file.");
        }
    }

    public async Task SendScreenData(
        string selectedDisplay,
        IEnumerable<string> displayNames,
        int screenWidth,
        int screenHeight)
    {
        var dto = new ScreenDataDto()
        {
            MachineName = Environment.MachineName,
            DisplayNames = displayNames,
            SelectedDisplay = selectedDisplay,
            ScreenWidth = screenWidth,
            ScreenHeight = screenHeight
        };
        await TrySendToViewer(dto, DtoType.ScreenData, ViewerConnectionId);
    }

    public async Task SendScreenSize(int width, int height)
    {
        var dto = new ScreenSizeDto(width, height);
        await TrySendToViewer(dto, DtoType.ScreenSize, ViewerConnectionId);
    }

    public async Task SendSessionMetrics(SessionMetricsDto metrics)
    {
        await TrySendToViewer(metrics, DtoType.SessionMetrics, ViewerConnectionId);
    }

    public async Task SendWindowsSessions()
    {
        if (OperatingSystem.IsWindows())
        {
            var dto = new WindowsSessionsDto(Win32Interop.GetActiveSessions());
            await TrySendToViewer(dto, DtoType.WindowsSessions, ViewerConnectionId);
        }
    }

    public void SetLastFrameReceived(DateTimeOffset timestamp)
    {
        _lastFrameReceived = timestamp;
        _framesSentSinceLastReceipt = 0;
    }

    public async Task<bool> WaitForViewer()
    {
        // Prevent publisher from overwhelming consumer bewteen receipts.
        var result = await WaitHelper.WaitForAsync(
            () => _framesSentSinceLastReceipt < 10,
            TimeSpan.FromSeconds(5));

        // Prevent viewer from getting too far behind.
        result &= await WaitHelper.WaitForAsync(
            () => _lastFrameSent - _lastFrameReceived < TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5));

        return result;
    }

    private async void AudioCapturer_AudioSampleReady(object? sender, byte[] sample)
    {
        await SendAudioSample(sample);
    }

    private void CalculateFps()
    {
        if (_fpsQueue.Count >= 2)
        {
            var sendTime = _fpsQueue.Last() - _fpsQueue.First();
            CurrentFps = _fpsQueue.Count / sendTime.TotalSeconds;
        }
        else
        {

            CurrentFps = _fpsQueue.Count;
        }
        _fpsQueue.Clear();
    }

    private async Task CalculateLatency()
    {
        var latencyResult = await _desktopHubConnection.CheckRoundtripLatency(ViewerConnectionId);
        if (latencyResult.IsSuccess)
        {
            _pingFailures = 0;
            IsResponsive = true;
            RoundTripLatency = latencyResult.Value;
        }
        else
        {
            _pingFailures++;
            if (_pingFailures > 3)
            {
                IsResponsive = false;
                _logger.LogWarning("Failed to check roundtrip latency: {reason}", latencyResult.Reason);
            }
        }
    }
    private void CalculateMbps()
    {
        if (_sentFrames.Count >= 2)
        {
            var sendTime = _sentFrames.Last().Timestamp - _sentFrames.First().Timestamp;
            var sentBits = (double)_sentFrames.Sum(x => x.FrameSize) / 1024 / 1024 * 8;
            CurrentMbps = sentBits / sendTime.TotalSeconds;
        }
        else if (_sentFrames.Count == 1)
        {
            CurrentMbps = _sentFrames.First().FrameSize / 1024 / 1024 * 8;
        }
        else
        {
            CurrentMbps = 0;
        }
        _sentFrames.Clear();
    }
    private async void ClipboardService_ClipboardTextChanged(object? sender, string clipboardText)
    {
        await SendClipboardText(clipboardText);
    }

    private async Task TrySendToViewer<T>(T dto, DtoType type, string viewerConnectionId)
    {
        try
        {
            if (!_desktopHubConnection.IsConnected)
            {
                _logger.LogWarning(
                    "Unable to send DTO type {type} because the app is disconnected from the server.", 
                    type);
                return;
            }

            foreach (var chunk in DtoChunker.ChunkDto(dto, type))
            {
                await _desktopHubConnection.SendDtoToViewer(chunk, viewerConnectionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending DTO type {type} to viewer connection ID {viewerId}.", 
                type,
                viewerConnectionId);
        }
    }
}
