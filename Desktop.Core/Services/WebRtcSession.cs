using MessagePack;
using Remotely.Shared.Utilities;
using Remotely.Shared.Models;
using Remotely.Shared.Models.RemoteControlDtos;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SIPSorcery.Net;
using System.Timers;
using System.Collections.Generic;
using System.Text.Json;

namespace Remotely.Desktop.Core.Services
{
    public class WebRtcSession : IDisposable
    {
        public WebRtcSession(Viewer viewer, IDtoMessageHandler rtcMessageHandler)
        {
            Viewer = viewer;
            RtcMessageHandler = rtcMessageHandler;
            DataChannelBufferMonitor = new Timer(500);
            DataChannelBufferMonitor.Elapsed += DataChannelBufferMonitor_Elapsed;
            DataChannelBufferMonitor.Start();
        }



        public event EventHandler<RTCIceCandidate> IceCandidateReady;

        public event EventHandler<RTCSessionDescriptionInit> LocalSdpReady;

        public ulong CurrentBuffer { get; private set; }
        public bool IsDataChannelOpen => CaptureChannel?.readyState == RTCDataChannelState.open;
        public bool IsPeerConnected => PeerSession?.connectionState == RTCPeerConnectionState.connected;

        private RTCDataChannel CaptureChannel { get; set; }
        private IceServerModel[] IceServers { get; set; }
        private RTCPeerConnection PeerSession { get; set; }
        private IDtoMessageHandler RtcMessageHandler { get; }
        private Timer DataChannelBufferMonitor { get; }
        private Viewer Viewer { get; }
        public void AddIceCandidate(string candidateJson)
        {
            var candidate = JsonSerializer.Deserialize<RTCIceCandidateInit>(candidateJson);
            PeerSession.addIceCandidate(candidate);
        }

        public void Dispose()
        {
            try
            {
                PeerSession?.DataChannels?.Clear();
                CaptureChannel?.close();
                DataChannelBufferMonitor?.Dispose();
            }
            catch { }

            PeerSession?.Dispose();
            GC.SuppressFinalize(this);
        }

        public Task Init(IceServerModel[] iceServers)
        {
            Logger.Write("Starting WebRTC connection.");

            IceServers = iceServers;

            var config = new RTCConfiguration()
            {
                iceServers = iceServers.Select(x => new RTCIceServer()
                {
                    credential = x.Credential,
                    credentialType = RTCIceCredentialType.password,
                    urls = x.Urls,
                    username = x.Username

                }).ToList()
            };

            PeerSession = new RTCPeerConnection(config);

            PeerSession.oniceconnectionstatechange += PeerSession_oniceconnectionstatechange;
            PeerSession.onicecandidate += PeerSession_onicecandidate;

            var dataChannelInit = new RTCDataChannelInit();
            CaptureChannel = PeerSession.createDataChannel("RemoteControl", dataChannelInit);

            CaptureChannel.onDatamessage += CaptureChannel_onDatamessage;
            CaptureChannel.onclose += CaptureChannel_onclose;
            CaptureChannel.onopen += CaptureChannel_onopen;
            CaptureChannel.onerror += CaptureChannel_onerror;

            var offer = PeerSession.createOffer(new RTCOfferOptions());
            LocalSdpReady?.Invoke(this, offer);

            return Task.CompletedTask;
        }


        public async Task SendDto<T>(T dto) where T : BaseDto
        {
            await CaptureChannel.sendasync(MessagePackSerializer.Serialize(dto));
        }

        public Task SetRemoteDescription(string type, string sdp)
        {
            if (!Enum.TryParse<RTCSdpType>(type, true, out var sdpMessageType))
            {
                Logger.Write("Unable to parse remote WebRTC description type.");
                return Task.CompletedTask;
            }

            PeerSession.setRemoteDescription(new RTCSessionDescriptionInit()
            {
                type = sdpMessageType,
                sdp = sdp
            });


            if (sdpMessageType == RTCSdpType.offer)
            {
                PeerSession.createAnswer(new RTCAnswerOptions());
            }

            return Task.CompletedTask;
        }

        private async void CaptureChannel_onDatamessage(byte[] obj)
        {
            await RtcMessageHandler.ParseMessage(Viewer, obj);
        }



        private async void CaptureChannel_onerror(string obj)
        {
            // Clear the queue when WebRTC state changes.
            Viewer.PendingSentFrames.Clear();
            Logger.Write($"DataChannel error: {obj}");
            await Init(IceServers);
        }

        private void CaptureChannel_onopen()
        {
            // Clear the queue when WebRTC state changes.
            Viewer.PendingSentFrames.Clear();
            Logger.Write("DataChannel opened.");
        }

        private async void CaptureChannel_onclose()
        {
            // Clear the queue when WebRTC state changes.
            Viewer.PendingSentFrames.Clear();
            Logger.Write("DataChannel closed.");
            await Init(IceServers);
        }

        private void PeerSession_oniceconnectionstatechange(RTCIceConnectionState newState)
        {
            // Clear the queue when WebRTC state changes.
            Viewer.PendingSentFrames.Clear();
            Logger.Write($"Ice state changed to {newState}.");
        }

        private void PeerSession_onicecandidate(RTCIceCandidate candidate)
        {
            Logger.Write("Ice candidate ready to send.");
            IceCandidateReady?.Invoke(this, candidate);
        }

        private void DataChannelBufferMonitor_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (CaptureChannel is not null)
            {
                CurrentBuffer = CaptureChannel.bufferedAmount;
            }
        }
    }
}
