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
                if (Context.Items.TryGetValue("SessionInfo", out var result))
                {
                    return (RCSessionInfo)result;
                }
                var newSession = new RCSessionInfo();
                Context.Items["SessionInfo"] = newSession;
                return newSession;
            }
        }

        private IHubContext<ViewerHub> ViewerHubContext { get; }

        private ConcurrentList<string> ViewerList => SessionInfo.ViewerList;

        public async Task DisconnectViewer(string viewerID, bool notifyViewer)
        {
            ViewerList.Remove(viewerID);

            if (notifyViewer)
            {
                await ViewerHubContext.Clients.Client(viewerID).SendAsync("ViewerRemoved");
            }
        }

        public IceServerModel[] GetIceServers()
        {
            return _appConfig.IceServers;
        }

        public string GetSessionID()
        {
            var random = new Random();
            var sessionId = "";

            while (string.IsNullOrWhiteSpace(sessionId) ||
                SessionInfoList.ContainsKey(sessionId))
            {
                for (var i = 0; i < 3; i++)
                {
                    sessionId += random.Next(0, 999).ToString().PadLeft(3, '0');
                }
            }
            
            Context.Items["SessionID"] = sessionId;

            SessionInfoList[Context.ConnectionId].AttendedSessionID = sessionId;

            return sessionId;
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
            SessionInfo.CasterSocketID = Context.ConnectionId;
            SessionInfo.StartTime = DateTimeOffset.Now;
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
            ViewerList.Add(viewerConnectionId);
            return Task.CompletedTask;
        }
    }
}
