import * as UI from "./UI.js";
import * as Utilities from "../Utilities.js";
import { MainRc } from "./Main.js";
import { IceServerModel } from "../Models/IceServerModel.js";

export class RtcSession {
    PeerConnection: RTCPeerConnection;
    DataChannel: RTCDataChannel;
    MessagePack: any = window['MessagePack'];
    Init(iceServers: IceServerModel[]) {

        this.PeerConnection = new RTCPeerConnection({
            iceServers: iceServers.map(x => {
                return {
                    urls: x.Url,
                    username: x.TurnUsername,
                    credential: x.TurnPassword,
                    credentialType: "password"
                }
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
            await MainRc.RCHubConnection.SendIceCandidate(ev.candidate);
        };
    }

    Disconnect() {
        this.PeerConnection.close();
    }
    async ReceiveRtcOffer(sdp: string) {
        await this.PeerConnection.setRemoteDescription({ type: "offer", sdp: sdp });
        await this.PeerConnection.setLocalDescription(await this.PeerConnection.createAnswer());
        await MainRc.RCHubConnection.SendRtcAnswer(this.PeerConnection.localDescription);
        console.log("Set RTC offer.");
    }
    async ReceiveCandidate(candidate: RTCIceCandidate) {
        await this.PeerConnection.addIceCandidate(candidate);
        console.log("Set ICE candidate.");
    }

    SendDto(dto: any) {
        this.DataChannel.send(this.MessagePack.encode(dto));
    }
}
