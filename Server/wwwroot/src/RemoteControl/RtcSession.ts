import * as UI from "./UI.js";
import * as Utilities from "./Utilities.js";
import { ViewerApp } from "./App.js";
import { IceServerModel } from "./Models/IceServerModel.js";

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

                UI.StreamVideoButton.setAttribute("hidden", "hidden");
                UI.ScreenViewer.removeAttribute("hidden");
                UI.VideoScreenViewer.setAttribute("hidden", "hidden");
            };
            this.DataChannel.onerror = (ev) => {
                console.log("Data channel error.", ev.error);
                UI.ConnectionP2PIcon.style.display = "none";
                UI.ConnectionRelayedIcon.style.display = "unset";

                UI.StreamVideoButton.setAttribute("hidden", "hidden");
                UI.ScreenViewer.removeAttribute("hidden");
                UI.VideoScreenViewer.setAttribute("hidden", "hidden");
            };
            this.DataChannel.onmessage = (ev) => {
                var data = ev.data as ArrayBuffer;
                ViewerApp.DtoMessageHandler.ParseBinaryMessage(data);
               
            };
            this.DataChannel.onopen = (ev) => {
                console.log("Data channel opened.");
                UI.ConnectionP2PIcon.style.display = "unset";
                UI.ConnectionRelayedIcon.style.display = "none";

                UI.StreamVideoButton.removeAttribute("hidden");

                if (ViewerApp.Settings.streamModeEnabled) {
                    UI.UpdateStreamingToggled(true);
                    ViewerApp.MessageSender.SendToggleWebRtcVideo(true);
                }
            };
        };
        this.PeerConnection.onconnectionstatechange = function (ev) {
            console.log("Connection state changed to " + this.connectionState);
        }

        this.PeerConnection.oniceconnectionstatechange = function (ev) {
            console.log("ICE connection state changed to " + this.iceConnectionState);
        }
        this.PeerConnection.onicecandidate = async (ev) => {
            console.log("ICE candidate ready: ", ev.candidate);
            await ViewerApp.ViewerHubConnection.SendIceCandidate(ev.candidate);
        };

        UI.VideoScreenViewer.onloadedmetadata = (ev) => {
            UI.VideoScreenViewer.play();
        }
        this.PeerConnection.ontrack = (event) => {
            if (event.track) {
                UI.VideoScreenViewer.srcObject = new MediaStream([event.track]);
            }
        };
    }

    Disconnect() {
        this.PeerConnection.close();
    }
    async ReceiveRtcOffer(sdp: string) {
        await this.PeerConnection.setRemoteDescription({ type: "offer", sdp: sdp });
        await this.PeerConnection.setLocalDescription(await this.PeerConnection.createAnswer());
        await ViewerApp.ViewerHubConnection.SendRtcAnswer(this.PeerConnection.localDescription);
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
