using Microsoft.AspNetCore.SignalR;
using Remotely.Server.Models;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Remotely.Server.Hubs
{
    public class CasterHub : Hub
    {
        private readonly IApplicationConfig _appConfig;
        private readonly IHubContext<AgentHub> _agentHubContext;
        private readonly ICircuitManager _circuitManager;

        public CasterHub(ICircuitManager circuitManager,
            IHubContext<ViewerHub> viewerHubContext,
            IHubContext<AgentHub> agentHubContext,
            IApplicationConfig appConfig)
        {
            _circuitManager = circuitManager;
            ViewerHubContext = viewerHubContext;
            _agentHubContext = agentHubContext;
            _appConfig = appConfig;
        }

        public static ConcurrentDictionary<string, RCSessionInfo> SessionInfoList { get; } = new ConcurrentDictionary<string, RCSessionInfo>();
        private RCSessionInfo SessionInfo
        {
            get
            {
                if (Context.Items.ContainsKey("SessionInfo"))
                {
                    return (RCSessionInfo)Context.Items["SessionInfo"];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Context.Items["SessionInfo"] = value;
            }
        }

        private IHubContext<ViewerHub> ViewerHubContext { get; }
        private List<string> ViewerList
        {
            get
            {
                if (!Context.Items.ContainsKey("ViewerList"))
                {
                    Context.Items["ViewerList"] = new List<string>();
                }
                return Context.Items["ViewerList"] as List<string>;
            }
        }
        public async Task DisconnectViewer(string viewerID, bool notifyViewer)
        {
            lock (ViewerList)
            {
                ViewerList.Remove(viewerID);
            }
            if (notifyViewer)
            {
                await ViewerHubContext.Clients.Client(viewerID).SendAsync("ViewerRemoved");
            }
        }

        public IceServerModel[] GetIceServers()
        {
            return _appConfig.IceServers;
        }

        public Task GetSessionID()
        {
            var random = new Random();
            var sessionID = "";
            for (var i = 0; i < 3; i++)
            {
                sessionID += random.Next(0, 999).ToString().PadLeft(3, '0');
            }
            Context.Items["SessionID"] = sessionID;

            SessionInfoList[Context.ConnectionId].AttendedSessionID = sessionID;

            return Clients.Caller.SendAsync("SessionID", sessionID);
        }

        public Task NotifyRequesterUnattendedReady(string browserHubConnectionID)
        {
            var deviceId = SessionInfoList[Context.ConnectionId].DeviceID;
            _circuitManager.InvokeOnConnection(browserHubConnectionID, CircuitEventName.UnattendedSessionReady, Context.ConnectionId, deviceId);
            return Task.CompletedTask;
        }

        public Task NotifyViewersRelaunchedScreenCasterReady(string[] viewerIDs)
        {
            return ViewerHubContext.Clients.Clients(viewerIDs).SendAsync("RelaunchedScreenCasterReady", Context.ConnectionId);
        }

        public override async Task OnConnectedAsync()
        {
            SessionInfo = new RCSessionInfo()
            {
                CasterSocketID = Context.ConnectionId,
                StartTime = DateTimeOffset.Now
            };
            SessionInfoList.AddOrUpdate(Context.ConnectionId, SessionInfo, (id, si) => SessionInfo);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            SessionInfoList.Remove(Context.ConnectionId, out _);

            if (SessionInfo.Mode == RemoteControlMode.Normal)
            {
                await ViewerHubContext.Clients.Clients(ViewerList).SendAsync("ScreenCasterDisconnected");
            }
            else if (SessionInfo.Mode == RemoteControlMode.Unattended)
            {
                if (ViewerList.Count > 0)
                {
                    await ViewerHubContext.Clients.Clients(ViewerList).SendAsync("Reconnecting");
                    await _agentHubContext.Clients.Client(SessionInfo.ServiceID).SendAsync("RestartScreenCaster", ViewerList, SessionInfo.ServiceID, Context.ConnectionId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public Task ReceiveDeviceInfo(string serviceID, string machineName, string deviceID)
        {
            SessionInfo.ServiceID = serviceID;
            SessionInfo.MachineName = machineName;
            SessionInfo.DeviceID = deviceID;
            return Task.CompletedTask;
        }

        public Task SendConnectionFailedToViewers(List<string> viewerIDs)
        {
            return ViewerHubContext.Clients.Clients(viewerIDs).SendAsync("ConnectionFailed");
        }

        public Task SendConnectionRequestDenied(string viewerID)
        {
            return ViewerHubContext.Clients.Client(viewerID).SendAsync("ConnectionRequestDenied");
        }

        public Task SendMessageToViewer(string viewerId, string message)
        {
            return ViewerHubContext.Clients.Client(viewerId).SendAsync("ShowMessage", message);
        }

        public Task SendCtrlAltDelToAgent()
        {
            return _agentHubContext.Clients.Client(SessionInfo.ServiceID).SendAsync("CtrlAltDel");
        }

        public Task SendDtoToBrowser(byte[] dto, string viewerId)
        {
            return ViewerHubContext.Clients.Client(viewerId).SendAsync("SendDtoToBrowser", dto);
        }

        public Task SendIceCandidateToBrowser(string candidate, int sdpMlineIndex, string sdpMid, string viewerID)
        {
            if (_appConfig.UseWebRtc)
            {
                return ViewerHubContext.Clients.Client(viewerID).SendAsync("ReceiveIceCandidate", candidate, sdpMlineIndex, sdpMid);
            }

            return Task.CompletedTask;
        }

        public Task SendRtcOfferToBrowser(string sdp, string viewerID, IceServerModel[] iceServers)
        {
            if (_appConfig.UseWebRtc)
            {
                return ViewerHubContext.Clients.Client(viewerID).SendAsync("ReceiveRtcOffer", sdp, iceServers);
            }

            return Task.CompletedTask;
        }

        public Task ViewerConnected(string viewerConnectionId)
        {
            lock (ViewerList)
            {
                ViewerList.Add(viewerConnectionId);
            }
            return Task.CompletedTask;
        }
    }
}
