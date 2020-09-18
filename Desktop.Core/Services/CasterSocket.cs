using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;
using Remotely.Desktop.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Shared.Utilities;
using System.Threading;
using Remotely.Desktop.Core.Models;
using Remotely.Shared.Models.RemoteControlDtos;

namespace Remotely.Desktop.Core.Services
{
    public class CasterSocket
    {
        public CasterSocket(
            IDtoMessageHandler messageHandler,
            IScreenCaster screenCastService)
        {
            MessageHandler = messageHandler;
            ScreenCaster = screenCastService;
        }

        public HubConnection Connection { get; private set; }
        public bool IsConnected => Connection?.State == HubConnectionState.Connected;
        private IDtoMessageHandler MessageHandler { get; }
        private IScreenCaster ScreenCaster { get; }
        public async Task<bool> Connect(string host)
        {
            try
            {
                if (Connection != null)
                {
                    await Connection.StopAsync();
                    await Connection.DisposeAsync();
                }
                Connection = new HubConnectionBuilder()
                    .WithUrl($"{host}/CasterHub")
                    .AddMessagePackProtocol()
                    .WithAutomaticReconnect()
                    .Build();

                ApplyConnectionHandlers();

                await Connection.StartAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        public async Task Disconnect()
        {
            await Connection.StopAsync();
            await Connection.DisposeAsync();
        }

        public async Task DisconnectAllViewers()
        {
            var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
            foreach (var viewer in conductor.Viewers.Values.ToList())
            {
                await DisconnectViewer(viewer, true);
            }
        }

        public async Task<IceServerModel[]> GetIceServers()
        {
            return await Connection.InvokeAsync<IceServerModel[]>("GetIceServers");
        }

        public async Task GetSessionID()
        {
            await Connection.SendAsync("GetSessionID");
        }
        public async Task NotifyRequesterUnattendedReady(string requesterID)
        {
            await Connection.SendAsync("NotifyRequesterUnattendedReady", requesterID);
        }

        public async Task NotifyViewersRelaunchedScreenCasterReady(string[] viewerIDs)
        {
            await Connection.SendAsync("NotifyViewersRelaunchedScreenCasterReady", viewerIDs);
        }

        public async Task SendAudioSample(byte[] buffer, string viewerID)
        {
            await Connection.SendAsync("SendAudioSample", buffer, viewerID);
        }

        public async Task SendClipboardText(string clipboardText, string viewerID)
        {
            await Connection.SendAsync("SendClipboardText", clipboardText, viewerID);
        }

        public async Task SendConnectionFailedToViewers(List<string> viewerIDs)
        {
            await Connection.SendAsync("SendConnectionFailedToViewers", viewerIDs);
        }

        public async Task SendConnectionRequestDenied(string viewerID)
        {
            await Connection.SendAsync("SendConnectionRequestDenied", viewerID);
        }

        public async Task SendCtrlAltDel()
        {
            await Connection.SendAsync("CtrlAltDel");
        }

        public async Task SendCursorChange(CursorInfo cursor, string viewerID)
        {
            await Connection.SendAsync("SendCursorChange", cursor, viewerID);
        }

        public async Task SendDeviceInfo(string serviceID, string machineName, string deviceID)
        {
            await Connection.SendAsync("ReceiveDeviceInfo", serviceID, machineName, deviceID);
        }

        public async Task SendIceCandidateToBrowser(string candidate, int sdpMlineIndex, string sdpMid, string viewerConnectionID)
        {
            await Connection.SendAsync("SendIceCandidateToBrowser", candidate, sdpMlineIndex, sdpMid, viewerConnectionID);
        }

        public async Task SendMachineName(string machineName, string viewerID)
        {
            await Connection.SendAsync("SendMachineName", machineName, viewerID);
        }

        public async Task SendRtcOfferToBrowser(string sdp, string viewerID, IceServerModel[] iceServers)
        {
            await Connection.SendAsync("SendRtcOfferToBrowser", sdp, viewerID, iceServers);
        }

        public async Task SendScreenCapture(byte[] imageBytes, string viewerID, int left, int top, int width, int height, int imageQuality)
        {
            for (var i = 0; i < imageBytes.Length; i += 50_000)
            {
                await Connection.SendAsync("SendScreenCapture",
                    imageBytes.Skip(i).Take(50_000).ToArray(),
                    viewerID,
                    left,
                    top,
                    width,
                    height,
                    imageQuality,
                    false);
            }
            await Connection.SendAsync("SendScreenCapture", Array.Empty<byte>(),
                viewerID,
                left,
                top,
                width,
                height,
                imageQuality,
                true);
        }

        public async Task SendScreenData(string selectedScreen, string[] displayNames, string viewerID)
        {
            await Connection.SendAsync("SendScreenDataToBrowser", selectedScreen, displayNames, viewerID);
        }

        public async Task SendScreenSize(int width, int height, string viewerID)
        {
            await Connection.SendAsync("SendScreenSize", width, height, viewerID);
        }

        public async Task DisconnectViewer(Viewer viewer, bool notifyViewer)
        {
            viewer.DisconnectRequested = true;
            viewer.Dispose();
            await Connection.SendAsync("DisconnectViewer", viewer.ViewerConnectionID, notifyViewer);
        }
        public async Task SendWindowsSessions(List<WindowsSession> windowsSessions, string viewerID)
        {
            await Connection.SendAsync("SendWindowsSessions", windowsSessions, viewerID);
        }

        private void ApplyConnectionHandlers()
        {
            // TODO: Remove circular dependencies and the need for static IServiceProvider instance
            // by emitting these events so other services can listen for them.
            var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
            Connection.Closed += (ex) =>
            {
                Logger.Write($"Connection closed.  Error: {ex?.Message}");
                return Task.CompletedTask;
            };

            Connection.On("Disconnect", async (string reason) =>
            {
                Logger.Write($"Disconnecting caster socket.  Reason: {reason}");
                await DisconnectAllViewers();
            });

            Connection.On("GetScreenCast", (string viewerID, string requesterName, bool notifyUser) =>
            {
                try
                {
                    ScreenCaster.BeginScreenCasting(new ScreenCastRequest()
                    { 
                        NotifyUser = notifyUser,
                        ViewerID = viewerID, 
                        RequesterName = requesterName 
                    });
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });


            Connection.On("ReceiveIceCandidate", (string candidate, int sdpMlineIndex, string sdpMid, string viewerID) =>
            {
                try
                {
                    if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                    {
                        viewer.RtcSession.AddIceCandidate(sdpMid, sdpMlineIndex, candidate);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }

            });

            Connection.On("ReceiveRtcAnswer", async (string sdp, string viewerID) =>
            {
                try
                {
                    if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                    {
                        await viewer.RtcSession.SetRemoteDescription("answer", sdp);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });

            Connection.On("RequestScreenCast", (string viewerID, string requesterName, bool notifyUser) =>
            {
                conductor.InvokeScreenCastRequested(new ScreenCastRequest() 
                { 
                    NotifyUser = notifyUser,
                    ViewerID = viewerID, 
                    RequesterName = requesterName 
                });
            });

            Connection.On("SendDtoToClient", (byte[] baseDto, string viewerConnectionId) =>
            {
                if (conductor.Viewers.TryGetValue(viewerConnectionId, out var viewer))
                {
                    MessageHandler.ParseMessage(viewer, baseDto);
                }
            });

            Connection.On("SessionID", (string sessionID) =>
            {
                conductor.InvokeSessionIDChanged(sessionID);
            });

            Connection.On("ViewerDisconnected", async (string viewerID) =>
            {
                await Connection.SendAsync("DisconnectViewer", viewerID, false);
                if (conductor.Viewers.TryGetValue(viewerID, out var viewer))
                {
                    viewer.DisconnectRequested = true;
                    viewer.Dispose();
                }
                conductor.InvokeViewerRemoved(viewerID);

            });
        }
    }
}
