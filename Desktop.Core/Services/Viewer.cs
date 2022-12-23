using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Models;
using Remotely.Desktop.Core.ViewModels;
using Remotely.Shared.Utilities;
using Remotely.Shared.Models;
using Remotely.Shared.Models.RemoteControlDtos;
using Remotely.Shared.Win32;
using System;
using System.Collections.Concurrent;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace Remotely.Desktop.Core.Services
{
    public class Viewer : IDisposable
    {
        public const int DefaultQuality = 80;

        private readonly ConcurrentQueue<DateTimeOffset> _fpsQueue = new();
        private readonly ConcurrentQueue<SentFrame> _receivedFrames = new();
        public Viewer(ICasterSocket casterSocket,
            IScreenCapturer screenCapturer,
            IClipboardService clipboardService,
            IAudioCapturer audioCapturer)
        {
            Capturer = screenCapturer;
            CasterSocket = casterSocket;
            ClipboardService = clipboardService;
            ClipboardService.ClipboardTextChanged += ClipboardService_ClipboardTextChanged;
            AudioCapturer = audioCapturer;
            AudioCapturer.AudioSampleReady += AudioCapturer_AudioSampleReady;
        }
        public IScreenCapturer Capturer { get; }
        public double CurrentFps { get; private set; }
        public double CurrentMbps { get; private set; }
        public bool DisconnectRequested { get; set; }
        public bool HasControl { get; set; } = true;
        public int ImageQuality { get; private set; } = DefaultQuality;
        public bool IsConnected => CasterSocket.IsConnected;

        public bool IsStalled
        {
            get
            {
                return PendingSentFrames.TryPeek(out var result) && DateTimeOffset.Now - result.Timestamp > TimeSpan.FromSeconds(15);
            }
        }

        public string Name { get; set; }
        public ConcurrentQueue<SentFrame> PendingSentFrames { get; } = new();
        public TimeSpan RoundTripLatency { get; private set; }

        public string ViewerConnectionID { get; set; }
        private IAudioCapturer AudioCapturer { get; }


        private ICasterSocket CasterSocket { get; }

        private IClipboardService ClipboardService { get; }

        public void ApplyAutoQuality()
        {
            if (ImageQuality < DefaultQuality)
            {
                ImageQuality = Math.Min(DefaultQuality, ImageQuality + 2);
            }

            // Limit FPS.
            _ = TaskHelper.DelayUntil(() =>
                !PendingSentFrames.TryPeek(out var result) || DateTimeOffset.Now - result.Timestamp > TimeSpan.FromMilliseconds(50),
                TimeSpan.FromSeconds(5));

            // Delay based on roundtrip time to prevent too many frames from queuing up on slow connections.
            _ = TaskHelper.DelayUntil(() => PendingSentFrames.Count < 1 / RoundTripLatency.TotalSeconds,
                TimeSpan.FromSeconds(5));

            // Wait until oldest pending frame is within the past 1 second.
            _ = TaskHelper.DelayUntil(() =>
                !PendingSentFrames.TryPeek(out var result) || DateTimeOffset.Now - result.Timestamp < TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5));


            Debug.WriteLine(
                $"Current Mbps: {CurrentMbps}.  " +
                $"Current FPS: {CurrentFps}.  " +
                $"Roundtrip Latency: {RoundTripLatency}.  " +
                $"Image Quality: {ImageQuality}");
        }

        public void CalculateFps()
        {
            _fpsQueue.Enqueue(Time.Now);

            while (_fpsQueue.TryPeek(out var oldestTime) &&
                Time.Now - oldestTime > TimeSpan.FromSeconds(1))
            {
                _fpsQueue.TryDequeue(out _);
            }

            CurrentFps = _fpsQueue.Count;
        }

        public void DequeuePendingFrame()
        {
            if (PendingSentFrames.TryDequeue(out var frame))
            {
                RoundTripLatency = Time.Now - frame.Timestamp;
                _receivedFrames.Enqueue(new SentFrame(frame.FrameSize));
            }
            while (_receivedFrames.TryPeek(out var oldestFrame) &&
                Time.Now - oldestFrame.Timestamp > TimeSpan.FromSeconds(1))
            {
                _receivedFrames.TryDequeue(out _);
            }
            CurrentMbps = (double)_receivedFrames.Sum(x => x.FrameSize) / 1024 / 1024 * 8;
        }

        public void Dispose()
        {
            DisconnectRequested = true;
            Disposer.TryDisposeAll(Capturer);
            GC.SuppressFinalize(this);
        }

        public async Task SendAudioSample(byte[] audioSample)
        {
            var dto = new AudioSampleDto(audioSample);
            await TrySendToViewer(() => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
        }

        public async Task SendClipboardText(string clipboardText)
        {
            var dto = new ClipboardTextDto(clipboardText);
            await TrySendToViewer(() => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
        }

        public async Task SendCtrlAltDel()
        {
            await CasterSocket.SendCtrlAltDelToAgent();
        }

        public async Task SendCursorChange(CursorInfo cursorInfo)
        {
            if (cursorInfo is null)
            {
                return;
            }

            var dto = new CursorChangeDto(cursorInfo.ImageBytes, cursorInfo.HotSpot.X, cursorInfo.HotSpot.Y, cursorInfo.CssOverride);
            await TrySendToViewer(() => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
        }
        public async Task SendFile(FileUpload fileUpload, CancellationToken cancelToken, Action<double> progressUpdateCallback)
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

                await TrySendToViewer(async () => await CasterSocket.SendDtoToViewer(fileDto, ViewerConnectionID));

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
                        Buffer = br.ReadBytes(50_000),
                        FileName = fileUpload.DisplayName,
                        MessageId = messageId
                    };

                    await TrySendToViewer(async () => await CasterSocket.SendDtoToViewer(fileDto, ViewerConnectionID));

                    progressUpdateCallback((double)fs.Position / fs.Length);
                }

                fileDto = new FileDto()
                {
                    EndOfFile = true,
                    FileName = fileUpload.DisplayName,
                    MessageId = messageId,
                    StartOfFile = false
                };

                await TrySendToViewer(async () => await CasterSocket.SendDtoToViewer(fileDto, ViewerConnectionID));

                progressUpdateCallback(1);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public async Task SendScreenCapture(CaptureFrame screenFrame)
        {
            
            PendingSentFrames.Enqueue(new SentFrame(screenFrame.EncodedImageBytes.Length));

            var left = screenFrame.Left;
            var top = screenFrame.Top;
            var width = screenFrame.Width;
            var height = screenFrame.Height;

            var chunks = screenFrame.EncodedImageBytes.Chunk(50_000).ToArray();
            var chunkCount = chunks.Length;

            for (var i = 0; i < chunkCount; i++)
            {
                var chunk = chunks[i];

                var dto = new CaptureFrameDto()
                {
                    Left = left,
                    Top = top,
                    Width = width,
                    Height = height,
                    EndOfFrame = i == chunkCount - 1,
                    Sequence = screenFrame.Sequence,
                    ImageBytes = chunk
                };

                await TrySendToViewer(() => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
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
            await TrySendToViewer(() => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
        }

        public async Task SendScreenSize(int width, int height)
        {
            var dto = new ScreenSizeDto(width, height);
            await TrySendToViewer(() => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
        }

        public async Task SendViewerConnected()
        {
            await CasterSocket.SendViewerConnected(ViewerConnectionID);
        }

        public async Task SendWindowsSessions()
        {
            if (EnvironmentHelper.IsWindows)
            {
                var dto = new WindowsSessionsDto(Win32Interop.GetActiveSessions());
                await TrySendToViewer(() => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
            }
        }

        private async void AudioCapturer_AudioSampleReady(object sender, byte[] sample)
        {
            await SendAudioSample(sample);
        }

        private async void ClipboardService_ClipboardTextChanged(object sender, string clipboardText)
        {
            await SendClipboardText(clipboardText);
        }

        private Task TrySendToViewer(Func<Task> websocketSend)
        {
            try
            {
                return websocketSend();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return Task.CompletedTask;
            }
        }
    }
}
