import * as UI from "./UI.js";
import { ViewerApp } from "./App.js";
import { When } from "./Utilities.js";
export class RtcSession {
    constructor() {
        this.MessagePack = window['MessagePack'];
    }
    Init(iceServers) {
        var servers = iceServers.map(x => {
            return {
                urls: x.Urls,
                username: x.Username,
                credential: x.Credential,
                credentialType: "password"
            };
        });
        this.PeerConnection = new RTCPeerConnection({
            iceServers: servers
        });
        this.PeerConnection.ondatachannel = (ev) => {
            console.log("Data channel received.");
            this.DataChannel = ev.channel;
            this.DataChannel.binaryType = "arraybuffer";
            this.DataChannel.onclose = (ev) => {
                console.log("Data channel closed.");
                UI.ConnectionP2PIcon.style.display = "none";
                UI.ConnectionRelayedIcon.style.display = "unset";
                UI.ScreenViewer.removeAttribute("hidden");
                UI.VideoScreenViewer.setAttribute("hidden", "hidden");
            };
            this.DataChannel.onerror = (ev) => {
                console.log("Data channel error.", ev.error);
                UI.ConnectionP2PIcon.style.display = "none";
                UI.ConnectionRelayedIcon.style.display = "unset";
                UI.ScreenViewer.removeAttribute("hidden");
                UI.VideoScreenViewer.setAttribute("hidden", "hidden");
            };
            this.DataChannel.onmessage = (ev) => {
                var data = ev.data;
                ViewerApp.DtoMessageHandler.ParseBinaryMessage(data);
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
            console.log("ICE candidate ready: ", ev.candidate);
            await ViewerApp.ViewerHubConnection.SendIceCandidate(ev.candidate);
        };
        UI.VideoScreenViewer.onloadedmetadata = (ev) => {
            UI.VideoScreenViewer.play();
        };
        this.PeerConnection.ontrack = (event) => {
            if (event.track) {
                UI.VideoScreenViewer.srcObject = new MediaStream([event.track]);
            }
        };
    }
    Disconnect() {
        this.PeerConnection.close();
    }
    async ReceiveRtcOffer(sdp) {
        await this.PeerConnection.setRemoteDescription({ type: "offer", sdp: sdp });
        await this.PeerConnection.setLocalDescription(await this.PeerConnection.createAnswer());
        await ViewerApp.ViewerHubConnection.SendRtcAnswer(this.PeerConnection.localDescription);
        console.log("Set RTC offer.");
    }
    async ReceiveCandidate(candidateJson) {
        When(() => !!this.PeerConnection).then(async () => {
            var rtcCandidate = JSON.parse(candidateJson);
            await this.PeerConnection.addIceCandidate(rtcCandidate);
            console.log("Set ICE candidate.", rtcCandidate);
        });
    }
    SendDto(dto) {
        this.DataChannel.send(this.MessagePack.encode(dto));
    }
}
//# sourceMappingURL=RtcSession.js.map