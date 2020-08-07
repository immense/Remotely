using MessagePack;
using Microsoft.MixedReality.WebRTC;
using Remotely.Desktop.Core.Models;
using Remotely.Shared.Models;
using Remotely.Shared.Models.RtcDtos;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Services
{
    public class WebRtcSession : IDisposable
    {
        public WebRtcSession(Viewer viewer, IRtcMessageHandler rtcMessageHandler)
        {
            Viewer = viewer;
            RtcMessageHandler = rtcMessageHandler;
        }

        public event EventHandler<IceCandidate> IceCandidateReady;

        public event EventHandler<SdpMessage> LocalSdpReady;

        public ulong CurrentBuffer { get; private set; }
        public bool IsDataChannelOpen => CaptureChannel?.State == DataChannel.ChannelState.Open;
        public bool IsPeerConnected => PeerSession?.IsConnected == true;
        private DataChannel CaptureChannel { get; set; }
        private IceServerModel[] IceServers { get; set; }
        private PeerConnection PeerSession { get; set; }
        private IRtcMessageHandler RtcMessageHandler { get; }
        private Transceiver Transceiver { get; set; }
        private ExternalVideoTrackSource VideoSource { get; set; }
        private Viewer Viewer { get; }
        public bool IsVideoTrackConnected
        {
            get
            {
                return PeerSession?.LocalVideoTracks?.FirstOrDefault()?.Source?.Enabled == true;
            }
        }

        public void AddIceCandidate(string sdpMid, int sdpMlineIndex, string candidate)
        {
            PeerSession.AddIceCandidate(new IceCandidate()
            {
                Content = candidate,
                SdpMid = sdpMid,
                SdpMlineIndex = sdpMlineIndex
            });
        }

        public void Dispose()
        {
            try
            {
                PeerSession?.Close();
                PeerSession?.Dispose();
            }
            catch { }
        }

        public async Task Init(IceServerModel[] iceServers)
        {
            Logger.Debug("Starting WebRTC connection.");

            IceServers = iceServers;

            PeerSession = new PeerConnection();

            var iceList = IceServers.Select(x => new IceServer()
            {
                Urls = { x.Url },
                TurnPassword = x.TurnPassword ?? string.Empty,
                TurnUserName = x.TurnUsername ?? string.Empty
            }).ToList();

            var config = new PeerConnectionConfiguration()
            {
                IceServers = iceList
            };

            await PeerSession.InitializeAsync(config);

            PeerSession.LocalSdpReadytoSend += PeerSession_LocalSdpReadytoSend; ;
            PeerSession.Connected += PeerConnection_Connected;
            PeerSession.IceStateChanged += PeerConnection_IceStateChanged;
            PeerSession.IceCandidateReadytoSend += PeerSession_IceCandidateReadytoSend; ;

            CaptureChannel = await PeerSession.AddDataChannelAsync("ScreenCapture", true, true);
            CaptureChannel.BufferingChanged += DataChannel_BufferingChanged;
            CaptureChannel.MessageReceived += CaptureChannel_MessageReceived;
            CaptureChannel.StateChanged += CaptureChannel_StateChanged;

            VideoSource = ExternalVideoTrackSource.CreateFromArgb32Callback(GetCaptureFrame);
            Transceiver = PeerSession.AddTransceiver(MediaKind.Video);
            Transceiver.LocalVideoTrack = LocalVideoTrack.CreateFromSource(VideoSource, new LocalVideoTrackInitConfig()
            {
                trackName = "ScreenCapture"
            });

            PeerSession.CreateOffer();
        }

        public void SendAudioSample(byte[] audioSample)
        {
            SendDto(new AudioSampleDto(audioSample));
        }

        public void SendCaptureFrame(int left, int top, int width, int height, byte[] imageBytes, long imageQuality)
        {
            for (var i = 0; i < imageBytes.Length; i += 50_000)
            {
                SendDto(new CaptureFrameDto()
                {
                    Left = left,
                    Top = top,
                    Width = width,
                    Height = height,
                    EndOfFrame = false,
                    ImageBytes = imageBytes.Skip(i).Take(50_000).ToArray(),
                    ImageQuality = imageQuality
                });
            }
            SendDto(new CaptureFrameDto()
            {
                Left = left,
                Top = top,
                Width = width,
                Height = height,
                EndOfFrame = true,
                ImageQuality = imageQuality
            });
        }

        public void SendClipboardText(string clipboardText)
        {
            SendDto(new ClipboardTextDto(clipboardText));
        }

        public void SendCursorChange(CursorInfo cursorInfo)
        {
            SendDto(new CursorChangeDto(cursorInfo.ImageBytes, cursorInfo.HotSpot.X, cursorInfo.HotSpot.Y, cursorInfo.CssOverride));
        }

        public void SendMachineName(string machineName)
        {
            SendDto(new MachineNameDto(machineName));
        }

        public void SendScreenData(string selectedScreen, string[] displayNames)
        {
            SendDto(new ScreenDataDto(selectedScreen, displayNames));
        }

        public void SendScreenSize(int width, int height)
        {
            SendDto(new ScreenSizeDto(width, height));
        }

        public void SendWindowsSessions(List<WindowsSession> windowsSessions)
        {
            SendDto(new WindowsSessionsDto(windowsSessions));
        }

        public async Task SetRemoteDescription(string type, string sdp)
        {
            if (!Enum.TryParse<SdpMessageType>(type, true, out var sdpMessageType))
            {
                Logger.Write("Unable to parse remote WebRTC description type.");
                return;
            }

            await PeerSession.SetRemoteDescriptionAsync(new SdpMessage()
            {
                Content = sdp,
                Type = sdpMessageType
            });

            if (sdpMessageType == SdpMessageType.Offer)
            {
                PeerSession.CreateAnswer();
            }
        }

        private async void CaptureChannel_MessageReceived(byte[] obj)
        {
            Logger.Debug($"DataChannel message received.  Size: {obj.Length}");
            await RtcMessageHandler.ParseMessage(obj);
        }

        private async void CaptureChannel_StateChanged()
        {
            Logger.Debug($"DataChannel state changed.  New State: {CaptureChannel.State}");
            if (CaptureChannel.State == DataChannel.ChannelState.Closed)
            {
                await Init(IceServers);
            }
        }

        private void DataChannel_BufferingChanged(ulong previous, ulong current, ulong limit)
        {
            CurrentBuffer = current;
        }

        private void GetCaptureFrame(in FrameRequest request)
        {
            using (var currentFrame = Viewer.Capturer.GetNextFrame())
            {
                var bitmapData = currentFrame.LockBits(
                       Viewer.Capturer.CurrentScreenBounds,
                       System.Drawing.Imaging.ImageLockMode.ReadOnly,
                       System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                try
                {
                    var frame = new Argb32VideoFrame()
                    {
                        data = bitmapData.Scan0,
                        height = (uint)currentFrame.Height,
                        width = (uint)currentFrame.Width,
                        stride = bitmapData.Stride
                    };
                    request.CompleteRequest(in frame);
                }
                finally
                {
                    currentFrame.UnlockBits(bitmapData);
                }

            }

        }

        private void PeerConnection_Connected()
        {
            Logger.Debug("PeerConnection connected.");
        }

        private void PeerConnection_IceStateChanged(IceConnectionState newState)
        {
            Logger.Debug($"Ice state changed to {newState}.");
        }

        private void PeerSession_IceCandidateReadytoSend(IceCandidate candidate)
        {
            Logger.Debug("Ice candidate ready to send.");
            IceCandidateReady?.Invoke(this, candidate);
        }
        private void PeerSession_LocalSdpReadytoSend(SdpMessage message)
        {
            Logger.Debug($"Local SDP ready.");
            LocalSdpReady?.Invoke(this, message);
        }
        private void SendDto<T>(T dto)
        {
            CaptureChannel.SendMessage(MessagePackSerializer.Serialize(dto));
        }
    }
}
