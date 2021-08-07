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
        public const int DefaultQuality = 75;
        private const double MaxLatency = 300;
        private const double MaxMbps = 9;
        private const int MinQuality = 20;
        private readonly ConcurrentQueue<double> _sentMegabits = new();

        private readonly ConcurrentQueue<DateTimeOffset> _sentTimestamps = new();

        public Viewer(ICasterSocket casterSocket,
                            IScreenCapturer screenCapturer,
            IClipboardService clipboardService,
            IWebRtcSessionFactory webRtcSessionFactory,
            IAudioCapturer audioCapturer)
        {
            Capturer = screenCapturer;
            CasterSocket = casterSocket;
            WebRtcSessionFactory = webRtcSessionFactory;
            ClipboardService = clipboardService;
            ClipboardService.ClipboardTextChanged += ClipboardService_ClipboardTextChanged;
            AudioCapturer = audioCapturer;
            AudioCapturer.AudioSampleReady += AudioCapturer_AudioSampleReady;
        }
        public IScreenCapturer Capturer { get; }
        public double CurrentMbps { get; private set; }
        public bool DisconnectRequested { get; set; }
        public EncoderParameters EncoderParams { get; private set; }
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

        public bool IsUsingWebRtc
        {
            get
            {
                return RtcSession?.IsPeerConnected == true && RtcSession?.IsDataChannelOpen == true;
            }
        }

        public bool IsUsingWebRtcVideo
        {
            get
            {
                return RtcSession?.IsPeerConnected == true && RtcSession?.IsVideoTrackConnected == true;
            }
        }

        public string Name { get; set; }
        public ConcurrentQueue<SentFrame> PendingSentFrames { get; } = new();
        public TimeSpan RoundTripLatency { get; private set; }
        public WebRtcSession RtcSession { get; set; }

        public string ViewerConnectionID { get; set; }
        private IAudioCapturer AudioCapturer { get; }


        private ICasterSocket CasterSocket { get; }

        private IClipboardService ClipboardService { get; }

        private IWebRtcSessionFactory WebRtcSessionFactory { get; }
        public void ApplyAutoQuality()
        {
            if (ImageQuality < DefaultQuality)
            {
                ImageQuality = Math.Min(DefaultQuality, ImageQuality + 2);
            }

            // Limit to 20 FPS.
            _ = TaskHelper.DelayUntil(() =>
                !PendingSentFrames.TryPeek(out var result) || DateTimeOffset.Now - result.Timestamp > TimeSpan.FromMilliseconds(50),
                TimeSpan.FromSeconds(5));

            // Wait for pending frames to be received.
            _ = TaskHelper.DelayUntil(() =>
                !PendingSentFrames.TryPeek(out var result) || DateTimeOffset.Now - result.Timestamp < TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5));

            // Limit max number of pending frames.
            _ = TaskHelper.DelayUntil(() => PendingSentFrames.Count < 5, TimeSpan.FromSeconds(5));

            if (RoundTripLatency.TotalMilliseconds > MaxLatency)
            {
                ImageQuality = Math.Max(MinQuality, (int)(MaxLatency / RoundTripLatency.TotalMilliseconds * ImageQuality));
            }

            if (CurrentMbps > MaxMbps)
            {
                var targetPercent = MaxMbps / CurrentMbps;
                ImageQuality = Math.Max(MinQuality, (int)(ImageQuality * targetPercent));
                Debug.WriteLine($"Current Mbps: {CurrentMbps}.  Setting quality to {ImageQuality}");
            }
        }

        public void DequeuePendingFrame()
        {
            if (PendingSentFrames.TryDequeue(out var frame))
            {
                RoundTripLatency = Time.Now - frame.Timestamp;
            }
        }

        public void Dispose()
        {
            DisconnectRequested = true;
            Disposer.TryDisposeAll(RtcSession, Capturer);
            GC.SuppressFinalize(this);
        }

        public async Task InitializeWebRtc()
        {
            try
            {
                var iceServers = await CasterSocket.GetIceServers();

                RtcSession = WebRtcSessionFactory.GetNewSession(this);

                if (RtcSession is null)
                {
                    return;
                }

                RtcSession.LocalSdpReady += async (sender, sdp) =>
                {
                    await CasterSocket.SendRtcOfferToBrowser(sdp.Content, ViewerConnectionID, iceServers);
                };
                RtcSession.IceCandidateReady += async (sender, candidate) =>
                {
                    await CasterSocket.SendIceCandidateToBrowser(candidate.Content, candidate.SdpMlineIndex, candidate.SdpMid, ViewerConnectionID);
                };

                await RtcSession.Init(iceServers);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public async Task SendAudioSample(byte[] audioSample)
        {
            var dto = new AudioSampleDto(audioSample);
            await SendToViewer(() => RtcSession.SendDto(dto),
                () => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
        }

        public async Task SendClipboardText(string clipboardText)
        {
            var dto = new ClipboardTextDto(clipboardText);
            await SendToViewer(() => RtcSession.SendDto(dto),
                () => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
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
            await SendToViewer(() => RtcSession.SendDto(dto),
                () => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
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

                await SendToViewer(async () => await RtcSession.SendDto(fileDto),
                    async () => await CasterSocket.SendDtoToViewer(fileDto, ViewerConnectionID));

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

                    await SendToViewer(
                        async () => await RtcSession.SendDto(fileDto),
                        async () => await CasterSocket.SendDtoToViewer(fileDto, ViewerConnectionID));

                    progressUpdateCallback((double)fs.Position / fs.Length);
                }

                fileDto = new FileDto()
                {
                    EndOfFile = true,
                    FileName = fileUpload.DisplayName,
                    MessageId = messageId,
                    StartOfFile = false
                };

                await SendToViewer(async () => await RtcSession.SendDto(fileDto),
                    async () => await CasterSocket.SendDtoToViewer(fileDto, ViewerConnectionID));

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

            for (var i = 0; i < screenFrame.EncodedImageBytes.Length; i += 50_000)
            {
                var dto = new CaptureFrameDto()
                {
                    Left = left,
                    Top = top,
                    Width = width,
                    Height = height,
                    EndOfFrame = false,
                    Sequence = screenFrame.Sequence,
                    ImageBytes = screenFrame.EncodedImageBytes.Skip(i).Take(50_000).ToArray()
                };

                await SendToViewer(
                      () => RtcSession.SendDto(dto),
                      () => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
            }

            var endOfFrameDto = new CaptureFrameDto()
            {
                Left = left,
                Top = top,
                Width = width,
                Height = height,
                EndOfFrame = true,
                Sequence = screenFrame.Sequence,
            };

            await SendToViewer(
                       () => RtcSession.SendDto(endOfFrameDto),
                       () => CasterSocket.SendDtoToViewer(endOfFrameDto, ViewerConnectionID));

            UpdateMbps(screenFrame.EncodedImageBytes.Length);
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
            await SendToViewer(() => RtcSession.SendDto(dto),
                () => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
        }

        public async Task SendScreenSize(int width, int height)
        {
            var dto = new ScreenSizeDto(width, height);
            await SendToViewer(() => RtcSession.SendDto(dto),
                 () => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
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
                await SendToViewer(() => RtcSession.SendDto(dto),
                    () => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
            }
        }
        public void ToggleWebRtcVideo(bool toggleOn)
        {
            RtcSession.ToggleWebRtcVideo(toggleOn);
        }

        private async void AudioCapturer_AudioSampleReady(object sender, byte[] sample)
        {
            await SendAudioSample(sample);
        }

        private async void ClipboardService_ClipboardTextChanged(object sender, string clipboardText)
        {
            await SendClipboardText(clipboardText);
        }

        private Task SendToViewer(Func<Task> webRtcSend, Func<Task> websocketSend)
        {
            try
            {
                if (IsUsingWebRtc)
                {
                    return webRtcSend();
                }
                else
                {
                    return websocketSend();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return Task.CompletedTask;
            }
        }

        private void UpdateMbps(int frameSize)
        {
            while (_sentMegabits.Count > 10)
            {
                _sentMegabits.TryDequeue(out _);
            }
            while (_sentTimestamps.Count > 10)
            {
                _sentTimestamps.TryDequeue(out _);
            }
            var mb = (double)frameSize / 1024 / 1024 * 8;
            _sentMegabits.Enqueue(mb);
            _sentTimestamps.Enqueue(Time.Now);
            // Don't calculate Mbps until we have a decent sample size.
            if (_sentMegabits.Count < 5)
            {
                return;
            }
            _sentTimestamps.TryPeek(out var oldestTime);
            CurrentMbps = _sentMegabits.Sum() / (Time.Now - oldestTime).TotalSeconds;
        }
    }
}
