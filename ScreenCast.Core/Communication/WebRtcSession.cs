using MessagePack;
using Microsoft.MixedReality.WebRTC;
using Remotely.ScreenCast.Core.Services;
using Remotely.Shared.Models;
using Remotely.Shared.Models.RtcDtos;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.ScreenCast.Core.Communication
{
    public class WebRtcSession : IDisposable
    {
        public WebRtcSession(IRtcMessageHandler rtcMessageHandler)
        {
            RtcMessageHandler = rtcMessageHandler;
        }

        public event EventHandler<(string candidate, int sdpMlineIndex, string sdpMid)> IceCandidateReady;

        public event EventHandler<string> LocalSdpReady;

        public ulong CurrentBuffer { get; private set; }
        public bool IsDataChannelOpen => CaptureChannel?.State == DataChannel.ChannelState.Open;
        public bool IsPeerConnected => PeerConnection?.IsConnected == true;
        private DataChannel CaptureChannel { get; set; }
        private IceServerModel[] IceServers { get; set; }
        private PeerConnection PeerConnection { get; set; }
        private IRtcMessageHandler RtcMessageHandler { get; }
        public void AddIceCandidate(string sdpMid, int sdpMlineIndex, string candidate)
        {
            PeerConnection.AddIceCandidate(sdpMid, sdpMlineIndex, candidate);
        }

        public void Dispose()
        {
            CaptureChannel?.Dispose();
            PeerConnection?.Dispose();
        }

        public async Task Init(IceServerModel[] iceServers)
        {
            Logger.Debug("Starting WebRTC connection.");

            IceServers = iceServers;

            PeerConnection = new PeerConnection();

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

            await PeerConnection.InitializeAsync(config);

            PeerConnection.LocalSdpReadytoSend += PeerConnection_LocalSdpReadytoSend;
            PeerConnection.Connected += PeerConnection_Connected;
            PeerConnection.IceStateChanged += PeerConnection_IceStateChanged;
            PeerConnection.IceCandidateReadytoSend += PeerConnection_IceCandidateReadytoSend;
            CaptureChannel = await PeerConnection.AddDataChannelAsync("ScreenCapture", true, true);
            CaptureChannel.BufferingChanged += DataChannel_BufferingChanged;
            CaptureChannel.MessageReceived += CaptureChannel_MessageReceived;
            CaptureChannel.StateChanged += CaptureChannel_StateChanged;
            PeerConnection.CreateOffer();
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

        public void SetRemoteDescription(string type, string sdp)
        {
            PeerConnection.SetRemoteDescription(type, sdp);
            if (type == "offer")
            {
                PeerConnection.CreateAnswer();
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

        private void PeerConnection_Connected()
        {
            Logger.Debug("PeerConnection connected.");
        }

        private void PeerConnection_IceCandidateReadytoSend(string candidate, int sdpMlineindex, string sdpMid)
        {
            Logger.Debug("Ice candidate ready to send.");
            IceCandidateReady?.Invoke(this, (candidate, sdpMlineindex, sdpMid));
        }

        private void PeerConnection_IceStateChanged(IceConnectionState newState)
        {
            Logger.Debug($"Ice state changed to {newState}.");
        }

        private void PeerConnection_LocalSdpReadytoSend(string type, string sdp)
        {
            Logger.Debug($"Local SDP ready.");
            LocalSdpReady?.Invoke(this, sdp);
        }
        private void SendDto<T>(T dto)
        {
            CaptureChannel.SendMessage(MessagePackSerializer.Serialize(dto));
        }
    }
}
