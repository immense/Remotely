import * as UI from "./UI.js";
import { MainRc } from "./Main.js";
export class RtcSession {
    constructor() {
        this.MessagePack = window['MessagePack'];
    }
    Init(iceServers) {
        this.PeerConnection = new RTCPeerConnection({
            iceServers: iceServers.map(x => {
                return {
                    urls: x.Url,
                    username: x.TurnUsername,
                    credential: x.TurnPassword,
                    credentialType: "password"
                };
            })
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
                var data = ev.data;
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
        };
        this.PeerConnection.oniceconnectionstatechange = function (ev) {
            console.log("ICE connection state changed to " + this.iceConnectionState);
        };
        this.PeerConnection.onicecandidate = async (ev) => {
            await MainRc.RCHubConnection.SendIceCandidate(ev.candidate);
        };
    }
    Disconnect() {
        this.PeerConnection.close();
    }
    async ReceiveRtcOffer(sdp) {
        await this.PeerConnection.setRemoteDescription({ type: "offer", sdp: sdp });
        await this.PeerConnection.setLocalDescription(await this.PeerConnection.createAnswer());
        await MainRc.RCHubConnection.SendRtcAnswer(this.PeerConnection.localDescription);
        console.log("Set RTC offer.");
    }
    async ReceiveCandidate(candidate) {
        await this.PeerConnection.addIceCandidate(candidate);
        console.log("Set ICE candidate.");
    }
    SendDto(dto) {
        this.DataChannel.send(this.MessagePack.encode(dto));
    }
}
//# sourceMappingURL=RtcSession.js.map