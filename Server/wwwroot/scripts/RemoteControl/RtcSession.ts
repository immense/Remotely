import * as UI from "./UI.js";
import * as Utilities from "../Utilities.js";
import { RemoteControl } from "./Main.js";


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
            this.DataChannel.onbufferedamountlow = (ev) => {
                console.log("Buffer amount low.");
            };
            this.DataChannel.onclose = (ev) => {
                console.log("Data channel closed.");
            };
            this.DataChannel.onerror = (ev) => {
                console.log("Data channel error.", ev.error);
            };
            this.DataChannel.onmessage = async (ev) => {
                var data = ev.data;

                if (ev.data.arrayBuffer) {
                    data = await ev.data.arrayBuffer();
                }
                console.log("WebRTC frame received. Size: " + data.byteLength);
                var frameInfo = this.MessagePack.decode(data) as FrameInfo;
                var url = window.URL.createObjectURL(new Blob([frameInfo.ImageBytes]));
                var img = document.createElement("img");
                img.onload = () => {
                    UI.Screen2DContext.drawImage(img, frameInfo.Left, frameInfo.Top, frameInfo.Width, frameInfo.Height);
                    window.URL.revokeObjectURL(url);
                };
                img.src = url;
            };
            this.DataChannel.onopen = (ev) => {
                console.log("Data channel opened.");
            };
        };
        this.PeerConnection.onconnectionstatechange = function (ev) {
            console.log("Connection state changed to " + this.connectionState);
        }

        this.PeerConnection.oniceconnectionstatechange = function (ev) {
            console.log("Connection state changed to " + this.iceConnectionState);
        }
        this.PeerConnection.onicecandidate = async (ev) => {
            await RemoteControl.RCBrowserSockets.SendIceCandidate(ev.candidate);
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
            await RemoteControl.RCBrowserSockets.SendRtcAnswer(this.PeerConnection.localDescription);
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
}
