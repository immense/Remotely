using MessagePack;
using Microsoft.MixedReality.WebRTC;
using Remotely.ScreenCast.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.ScreenCast.Core.Communication
{
    public class WebRtcSession
    {
        public event EventHandler<(string candidate, int sdpMlineIndex, string sdpMid)> IceCandidateReady;

        public event EventHandler<string> LocalSdpReady;

        public ulong CurrentBuffer { get; private set; }

        public bool IsDataChannelOpen => CaptureChannel?.State == DataChannel.ChannelState.Open;
        private DataChannel CaptureChannel { get; set; }
        private PeerConnection PeerConnection { get; set; }
        public void AddIceCandidate(string sdpMid, int sdpMlineIndex, string candidate)
        {
            PeerConnection.AddIceCandidate(sdpMid, sdpMlineIndex, candidate);
        }

        public async Task Init()
        {
            PeerConnection = new PeerConnection();

            var config = new PeerConnectionConfiguration()
            {
                IceServers = new List<IceServer>
                {
                    new IceServer{ Urls = { "stun:stun.l.google.com:19302" } },
                    new IceServer{ Urls = { "stun:stun4.l.google.com:19302" } }
                }
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

        public void SendCaptureFrame(int left, int top, int width, int height, byte[] imageBytes)
        {
            CaptureChannel.SendMessage(MessagePackSerializer.Serialize(new FrameInfo()
            {
                Left = left,
                Top = top,
                Width = width,
                Height = height,
                ImageBytes = imageBytes
            }));
        }

        public void SetRemoteDescription(string type, string sdp)
        {
            PeerConnection.SetRemoteDescription(type, sdp);
            if (type == "offer")
            {
                PeerConnection.CreateAnswer();
            }
        }

        private void CaptureChannel_MessageReceived(byte[] obj)
        {
            Debug.WriteLine($"DataChannel message received.  Size: {obj.Length}");
        }

        private void CaptureChannel_StateChanged()
        {
            Debug.WriteLine($"DataChannel state changed.  New State: {CaptureChannel.State}");
        }

        private void DataChannel_BufferingChanged(ulong previous, ulong current, ulong limit)
        {
            Debug.WriteLine($"DataChannel buffering changed.  Previous: {previous}.  Current: {current}.  Limit: {limit}.");
            CurrentBuffer = current;
        }

        private void PeerConnection_Connected()
        {
            Debug.WriteLine("PeerConnection connected.");
        }

        private void PeerConnection_IceCandidateReadytoSend(string candidate, int sdpMlineindex, string sdpMid)
        {
            Debug.WriteLine("Ice candidate ready to send.");
            IceCandidateReady?.Invoke(this, (candidate, sdpMlineindex, sdpMid));
        }

        private void PeerConnection_IceStateChanged(IceConnectionState newState)
        {
            Debug.WriteLine($"Ice state changed to {newState}.");
        }

        private void PeerConnection_LocalSdpReadytoSend(string type, string sdp)
        {
            Debug.WriteLine($"Local SDP ready.");
            LocalSdpReady?.Invoke(this, sdp);
        }
    }
}
