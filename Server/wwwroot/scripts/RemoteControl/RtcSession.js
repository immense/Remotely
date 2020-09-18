import * as UI from "./UI.js";
import { MainViewer } from "./Main.js";
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
                UI.StreamVideoButton.setAttribute("hidden", "hidden");
                UI.ScreenViewer.removeAttribute("hidden");
                UI.QualityButton.removeAttribute("hidden");
                UI.VideoScreenViewer.setAttribute("hidden", "hidden");
            };
            this.DataChannel.onerror = (ev) => {
                console.log("Data channel error.", ev.error);
                UI.ConnectionP2PIcon.style.display = "none";
                UI.ConnectionRelayedIcon.style.display = "unset";
                UI.StreamVideoButton.setAttribute("hidden", "hidden");
                UI.ScreenViewer.removeAttribute("hidden");
                UI.QualityButton.removeAttribute("hidden");
                UI.VideoScreenViewer.setAttribute("hidden", "hidden");
            };
            this.DataChannel.onmessage = async (ev) => {
                var data = ev.data;
                MainViewer.DtoMessageHandler.ParseBinaryMessage(data);
            };
            this.DataChannel.onopen = (ev) => {
                console.log("Data channel opened.");
                UI.ConnectionP2PIcon.style.display = "unset";
                UI.ConnectionRelayedIcon.style.display = "none";
                UI.StreamVideoButton.removeAttribute("hidden");
            };
        };
        this.PeerConnection.onconnectionstatechange = function (ev) {
            console.log("Connection state changed to " + this.connectionState);
        };
        this.PeerConnection.oniceconnectionstatechange = function (ev) {
            console.log("ICE connection state changed to " + this.iceConnectionState);
        };
        this.PeerConnection.onicecandidate = async (ev) => {
            await MainViewer.ViewerHubConnection.SendIceCandidate(ev.candidate);
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
        await MainViewer.ViewerHubConnection.SendRtcAnswer(this.PeerConnection.localDescription);
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