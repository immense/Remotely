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

namespace Remotely.Desktop.Core.Services
{
    public class Viewer : IDisposable
    {
        private long _bytesSent;
        private TimeSpan _timeSpentSending = TimeSpan.Zero;

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

        public bool DisconnectRequested { get; set; }
        public EncoderParameters EncoderParams { get; private set; }
        public bool HasControl { get; set; } = true;
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

        public double AverageBytesPerSecond { get; set; }
        public ConcurrentQueue<SentFrame> PendingSentFrames { get; } = new();

        public WebRtcSession RtcSession { get; set; }

        public string ViewerConnectionID { get; set; }
        private IAudioCapturer AudioCapturer { get; }

        private ICasterSocket CasterSocket { get; }

        private IClipboardService ClipboardService { get; }

        private IWebRtcSessionFactory WebRtcSessionFactory { get; }

        public void Dispose()
        {
            DisconnectRequested = true;
            Disposer.TryDisposeAll(new IDisposable[]
            {
                RtcSession,
                Capturer
            });
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

        public async Task SendMachineName(string machineName)
        {
            var dto = new MachineNameDto(machineName);
            await SendToViewer(() => RtcSession.SendDto(dto),
                () => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
        }

        public async Task SendScreenCapture(CaptureFrame screenFrame)
        {
            PendingSentFrames.Enqueue(new SentFrame(DateTimeOffset.Now, screenFrame.EncodedImageBytes.Length));

            var left = screenFrame.Left;
            var top = screenFrame.Top;
            var width = screenFrame.Width;
            var height = screenFrame.Height;

            var sw = Stopwatch.StartNew();

            for (var i = 0; i < screenFrame.EncodedImageBytes.Length; i += 50_000)
            {
                var dto = new CaptureFrameDto()
                {
                    Left = left,
                    Top = top,
                    Width = width,
                    Height = height,
                    EndOfFrame = false,
                    ImageBytes = screenFrame.EncodedImageBytes.Skip(i).Take(50_000).ToArray()
                };

                await SendToViewer(() => RtcSession.SendDto(dto),
                    () => CasterSocket.SendDtoToViewer(dto, ViewerConnectionID));
            }

            var endOfFrameDto = new CaptureFrameDto()
            {
                Left = left,
                Top = top,
                Width = width,
                Height = height,
                EndOfFrame = true
            };

            await SendToViewer(
                () => RtcSession.SendDto(endOfFrameDto),
                () => CasterSocket.SendDtoToViewer(endOfFrameDto, ViewerConnectionID));

            sw.Stop();

            _bytesSent += screenFrame.EncodedImageBytes.Length;
            _timeSpentSending += sw.Elapsed;


            AverageBytesPerSecond = _bytesSent / _timeSpentSending.TotalSeconds;

            Debug.WriteLine($"Mbps: {AverageBytesPerSecond / 1024 / 1024 * 8}");
        }

        public async Task SendScreenData(string selectedScreen, string[] displayNames)
        {
            var dto = new ScreenDataDto(selectedScreen, displayNames);
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

        public void ThrottleIfNeeded()
        {
            TaskHelper.DelayUntil(() =>
                !PendingSentFrames.TryPeek(out var result) || DateTimeOffset.Now - result.Timestamp < TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(10));
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
    }
}
