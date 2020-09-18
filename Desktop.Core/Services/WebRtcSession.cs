using MessagePack;
using Microsoft.MixedReality.WebRTC;
using Remotely.Desktop.Core.Models;
using Remotely.Shared.Helpers;
using Remotely.Shared.Models;
using Remotely.Shared.Models.RemoteControlDtos;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Services
{
    public class WebRtcSession : IDisposable
    {
        public WebRtcSession(Viewer viewer, IDtoMessageHandler rtcMessageHandler)
        {
            Viewer = viewer;
            RtcMessageHandler = rtcMessageHandler;
        }

        public event EventHandler<IceCandidate> IceCandidateReady;

        public event EventHandler<SdpMessage> LocalSdpReady;

        public ulong CurrentBuffer { get; private set; }
        public bool IsDataChannelOpen => CaptureChannel?.State == DataChannel.ChannelState.Open;
        public bool IsPeerConnected => PeerSession?.IsConnected == true;
        public bool IsVideoTrackConnected
        {
            get
            {
                return Transceiver?.LocalVideoTrack?.Enabled == true;
            }
        }

        private DataChannel CaptureChannel { get; set; }
        private IceServerModel[] IceServers { get; set; }
        private PeerConnection PeerSession { get; set; }
        private IDtoMessageHandler RtcMessageHandler { get; }
        private Transceiver Transceiver { get; set; }
        private ExternalVideoTrackSource VideoSource { get; set; }
        private Viewer Viewer { get; }
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
                // Unable to exit process until DataChannel is removed/disposed,
                // and this throws internally (at least in 2.0 version).
                PeerSession?.RemoveDataChannel(CaptureChannel);
            }
            catch { }
            Disposer.TryDisposeAll(new IDisposable[]
            {
                PeerSession,
                Transceiver?.LocalVideoTrack,
                VideoSource
            });
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

            PeerSession.CreateOffer();
        }

        public void SendDto<T>(T dto) where T : BaseDto
        {
            CaptureChannel.SendMessage(MessagePackSerializer.Serialize(dto));
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

        public void ToggleWebRtcVideo(bool toggleOn)
        {
            if (Transceiver?.LocalVideoTrack != null)
            {
                Transceiver.LocalVideoTrack.Dispose();
                Transceiver.LocalVideoTrack = null;
            }

            if (toggleOn)
            {
                Transceiver.LocalVideoTrack = LocalVideoTrack.CreateFromSource(VideoSource, new LocalVideoTrackInitConfig()
                {
                    trackName = "ScreenCapture"
                });
            }
        }

        private async void CaptureChannel_MessageReceived(byte[] obj)
        {
            Logger.Debug($"DataChannel message received.  Size: {obj.Length}");
            await RtcMessageHandler.ParseMessage(Viewer, obj);
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
            try
            {
                using (var currentFrame = Viewer.Capturer.GetNextFrame())
                {
                    if (currentFrame == null)
                    {
                        return;
                    }

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
            catch (Exception ex)
            {
                Logger.Write(ex);
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
    }
}
