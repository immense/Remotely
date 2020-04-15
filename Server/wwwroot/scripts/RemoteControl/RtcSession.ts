import * as UI from "./UI.js";
import * as Utilities from "../Utilities.js";
import { MainRc } from "./Main.js";

export class RtcSession {
    PeerConnection: RTCPeerConnection;
    DataChannel: RTCDataChannel;
    MessagePack: any = window['MessagePack'];
    Init() {
        this.PeerConnection = new RTCPeerConnection({
            iceServers: [
                { urls: "stun: stun.l.google.com:19302" },
                { urls: "stun:stun4.l.google.com:19302" }
            ]
        });
        
        this.PeerConnection.ondatachannel = (ev) => {
            console.log("Data channel received.");
            this.DataChannel = ev.channel;
            this.DataChannel.binaryType = "arraybuffer";
            this.DataChannel.onclose = (ev) => {
                console.log("Data channel closed.");
                UI.ConnectionP2PIcon.style.display = "none";
                UI.ConnectionRelayedIcon.style.display = "unset";
            };
            this.DataChannel.onerror = (ev) => {
                console.log("Data channel error.", ev.error);
                UI.ConnectionP2PIcon.style.display = "none";
                UI.ConnectionRelayedIcon.style.display = "unset";
            };
            this.DataChannel.onmessage = async (ev) => {
                var data = ev.data as ArrayBuffer;
                MainRc.RtcMessageHandler.ParseBinaryMessage(data);
               
            };
            this.DataChannel.onopen = (ev) => {
                console.log("Data channel opened.");
                UI.ConnectionP2PIcon.style.display = "unset";
                UI.ConnectionRelayedIcon.style.display = "none";
            };
        };
        this.PeerConnection.onconnectionstatechange = function (ev) {
            console.log("Connection state changed to " + this.connectionState);
        }

        this.PeerConnection.oniceconnectionstatechange = function (ev) {
            console.log("ICE connection state changed to " + this.iceConnectionState);
        }
        this.PeerConnection.onicecandidate = async (ev) => {
            await MainRc.RCBrowserSockets.SendIceCandidate(ev.candidate);
        };
    }

    Disconnect() {
        this.PeerConnection.close();
    }
    async ReceiveRtcOffer(sdp: string) {
        await this.PeerConnection.setRemoteDescription({ type: "offer", sdp: sdp });

        Utilities.When(() => {
            return this.PeerConnection.remoteDescription.sdp.length > 0;
        }).then(async () => {
            await this.PeerConnection.setLocalDescription(await this.PeerConnection.createAnswer());
            await MainRc.RCBrowserSockets.SendRtcAnswer(this.PeerConnection.localDescription);
        })
    }
    async ReceiveCandidate(candidate: RTCIceCandidate) {
        Utilities.When(() => {
            return this.PeerConnection.remoteDescription.sdp.length > 0;
        }).then(async () => {
            await this.PeerConnection.addIceCandidate(candidate);
            console.log("Set ICE candidate.");
        })
    }

    SendDto(dto: any) {
        this.DataChannel.send(this.MessagePack.encode(dto));
    }
}
