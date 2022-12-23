using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Services
{
    public interface ICasterSocket
    {
        HubConnection Connection { get; }
        bool IsConnected { get; }

        Task<bool> Connect(string host);
        Task Disconnect();
        Task DisconnectAllViewers();
        Task DisconnectViewer(Viewer viewer, bool notifyViewer);
        Task<string> GetSessionID();
        Task NotifyRequesterUnattendedReady(string requesterID);
        Task NotifyViewersRelaunchedScreenCasterReady(string[] viewerIDs);
        Task SendConnectionFailedToViewers(List<string> viewerIDs);
        Task SendConnectionRequestDenied(string viewerID);
        Task SendCtrlAltDelToAgent();
        Task SendDeviceInfo(string serviceID, string machineName, string deviceID);
        Task SendDtoToViewer<T>(T dto, string viewerId);
        Task SendMessageToViewer(string viewerID, string message);
        Task SendViewerConnected(string viewerConnectionId);
    }

    public class CasterSocket : ICasterSocket
    {
        public CasterSocket(
            IdleTimer idleTimer,
            IDtoMessageHandler messageHandler,
            IScreenCaster screenCastService,
            IRemoteControlAccessService remoteControlAccessService)
        {
            IdleTimer = idleTimer;
            MessageHandler = messageHandler;
            ScreenCaster = screenCastService;
            RemoteControlAccessService = remoteControlAccessService;
        }

        public HubConnection Connection { get; private set; }
        public bool IsConnected => Connection?.State == HubConnectionState.Connected;
        private IdleTimer IdleTimer { get; }
        private IDtoMessageHandler MessageHandler { get; }
        private IScreenCaster ScreenCaster { get; }
        private IRemoteControlAccessService RemoteControlAccessService { get; }

        public async Task<bool> Connect(string host)
        {
            try
            {
                if (Connection?.State == HubConnectionState.Connected)
                {
                    try
                    {
                        await Connection.StopAsync();
                        await Connection.DisposeAsync();
                    }
                    catch { }
                }
                Connection = new HubConnectionBuilder()
                    .WithUrl($"{host.Trim().TrimEnd('/')}/hubs/desktop")
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
            try
            {
                await Connection.StopAsync();
                await Connection.DisposeAsync();
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error disconnecting websocket.");
            }
        }

        public async Task DisconnectAllViewers()
        {
            var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
            foreach (var viewer in conductor.Viewers.Values.ToList())
            {
                await DisconnectViewer(viewer, true);
            }
        }

        public Task DisconnectViewer(Viewer viewer, bool notifyViewer)
        {
            viewer.DisconnectRequested = true;
            viewer.Dispose();
            return Connection.SendAsync("DisconnectViewer", viewer.ViewerConnectionID, notifyViewer);
        }

        public async Task<string> GetSessionID()
        {
            return await Connection.InvokeAsync<string>("GetSessionID");
        }
        public Task NotifyRequesterUnattendedReady(string requesterID)
        {
            return Connection.SendAsync("NotifyRequesterUnattendedReady", requesterID);
        }

        public Task NotifyViewersRelaunchedScreenCasterReady(string[] viewerIDs)
        {
            return Connection.SendAsync("NotifyViewersRelaunchedScreenCasterReady", viewerIDs);
        }

        public Task SendConnectionFailedToViewers(List<string> viewerIDs)
        {
            return Connection.SendAsync("SendConnectionFailedToViewers", viewerIDs);
        }

        public Task SendConnectionRequestDenied(string viewerID)
        {
            return Connection.SendAsync("SendConnectionRequestDenied", viewerID);
        }

        public Task SendMessageToViewer(string viewerID, string message)
        {
            return Connection.SendAsync("SendMessageToViewer", viewerID, message);
        }

        public Task SendCtrlAltDelToAgent()
        {
            return Connection.SendAsync("SendCtrlAltDelToAgent");
        }

        public Task SendDeviceInfo(string serviceID, string machineName, string deviceID)
        {
            return Connection.SendAsync("ReceiveDeviceInfo", serviceID, machineName, deviceID);
        }

        public Task SendDtoToViewer<T>(T dto, string viewerId)
        {
            var serializedDto = MessagePack.MessagePackSerializer.Serialize(dto);
            return Connection.SendAsync("SendDtoToBrowser", serializedDto, viewerId);
        }

        public Task SendViewerConnected(string viewerConnectionId)
        {
            return Connection.SendAsync("ViewerConnected", viewerConnectionId);
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

            Connection.On("GetScreenCast", async (
                string viewerID,
                string requesterName,
                bool notifyUser,
                bool enforceAttendedAccess,
                string organizationName) =>
            {
                try
                {
                    if (enforceAttendedAccess)
                    {
                        await SendMessageToViewer(viewerID, "Asking user for permission...");

                        IdleTimer.Stop();
                        var result = await RemoteControlAccessService.PromptForAccess(requesterName, organizationName);
                        IdleTimer.Start();

                        if (!result)
                        {
                            await SendConnectionRequestDenied(viewerID);
                            return;
                        }
                    }

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
