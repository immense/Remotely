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
        }



        public event EventHandler<RTCIceCandidate> IceCandidateReady;

        public event EventHandler<RTCSessionDescriptionInit> LocalSdpReady;

        public bool IsDataChannelOpen => CaptureChannel?.readyState == RTCDataChannelState.open;
        public bool IsPeerConnected => PeerSession?.connectionState == RTCPeerConnectionState.connected;

        private RTCDataChannel CaptureChannel { get; set; }
        private IceServerModel[] IceServers { get; set; }
        private RTCPeerConnection PeerSession { get; set; }
        private IDtoMessageHandler RtcMessageHandler { get; }
        private Viewer Viewer { get; }

        public void AddIceCandidate(string candidateJson)
        {
            if (RTCIceCandidateInit.TryParse(candidateJson, out var rtcCandidate))
            {
                PeerSession.addIceCandidate(rtcCandidate);
            }
            else
            {
                Logger.Write("End of ICE candidates.  Adding null candidate.");
                PeerSession.addIceCandidate(rtcCandidate);
            }
        }

        public void Dispose()
        {
            try
            {
                PeerSession?.DataChannels?.Clear();
                CaptureChannel?.close();
            }
            catch { }

            PeerSession?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Init(IceServerModel[] iceServers)
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

            var dataChannelInit = new RTCDataChannelInit()
            {
                ordered = true
            };
            CaptureChannel = await PeerSession.createDataChannel("RemoteControl", dataChannelInit);

            CaptureChannel.onmessage += CaptureChannel_onmessage; ;
            CaptureChannel.onclose += CaptureChannel_onclose;
            CaptureChannel.onopen += CaptureChannel_onopen;
            CaptureChannel.onerror += CaptureChannel_onerror;

            var offer = PeerSession.createOffer(new RTCOfferOptions());
            LocalSdpReady?.Invoke(this, offer);
        }



        public Task SendDto<T>(T dto) where T : BaseDto
        {
            CaptureChannel.send(MessagePackSerializer.Serialize(dto));

            TaskHelper.DelayUntil(() => CaptureChannel.bufferedAmount < 64_000, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
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

        private async void CaptureChannel_onmessage(RTCDataChannel dc, DataChannelPayloadProtocols protocol, byte[] data)
        {
            await RtcMessageHandler.ParseMessage(Viewer, data);
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
    }
}
