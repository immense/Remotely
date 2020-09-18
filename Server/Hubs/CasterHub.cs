using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using Remotely.Shared.Models;
using Remotely.Server.Models;
using Remotely.Server.Services;

namespace Remotely.Server.Hubs
{
    public class CasterHub : Hub
    {
        public CasterHub(IHubContext<BrowserHub> browserHub,
            IHubContext<ViewerHub> viewerHubContext,
            IHubContext<AgentHub> agentHubContext,
            ApplicationConfig appConfig)
        {
            BrowserHubContext = browserHub;
            ViewerHubContext = viewerHubContext;
            AgentHubContext = agentHubContext;
            AppConfig = appConfig;
        }

        public static ConcurrentDictionary<string, RCSessionInfo> SessionInfoList { get; } = new ConcurrentDictionary<string, RCSessionInfo>();
        public ApplicationConfig AppConfig { get; }
        private IHubContext<AgentHub> AgentHubContext { get; }
        private IHubContext<BrowserHub> BrowserHubContext { get; }
        private IHubContext<ViewerHub> ViewerHubContext { get; }

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

        public Task CtrlAltDel()
        {
            return AgentHubContext.Clients.Client(SessionInfo.ServiceID).SendAsync("CtrlAltDel");
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
            return AppConfig.IceServers;
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
            return BrowserHubContext.Clients.Client(browserHubConnectionID).SendAsync("UnattendedSessionReady", Context.ConnectionId);
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

            if (SessionInfo.Mode == Shared.Enums.RemoteControlMode.Normal)
            {
                await ViewerHubContext.Clients.Clients(ViewerList).SendAsync("ScreenCasterDisconnected");
            }
            else if (SessionInfo.Mode == Shared.Enums.RemoteControlMode.Unattended)
            {
                if (ViewerList.Count > 0)
                {
                    await ViewerHubContext.Clients.Clients(ViewerList).SendAsync("Reconnecting");
                    await AgentHubContext.Clients.Client(SessionInfo.ServiceID).SendAsync("RestartScreenCaster", ViewerList, SessionInfo.ServiceID, Context.ConnectionId);
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

        public Task SendDtoToBrowser(byte[] dto, string viewerId)
        {
            return ViewerHubContext.Clients.Client(viewerId).SendAsync("SendDtoToBrowser", dto);
        }

        public Task SendClipboardText(string clipboardText, string viewerID)
        {
            return ViewerHubContext.Clients.Client(viewerID).SendAsync("ClipboardTextChanged", clipboardText);
        }

        public Task SendConnectionFailedToViewers(List<string> viewerIDs)
        {
            return ViewerHubContext.Clients.Clients(viewerIDs).SendAsync("ConnectionFailed");
        }

        public Task SendConnectionRequestDenied(string viewerID)
        {
            return ViewerHubContext.Clients.Client(viewerID).SendAsync("ConnectionRequestDenied");
        }

        public Task SendCursorChange(CursorInfo cursor, string viewerID)
        {
            return ViewerHubContext.Clients.Client(viewerID).SendAsync("CursorChange", cursor);
        }

        public Task SendIceCandidateToBrowser(string candidate, int sdpMlineIndex, string sdpMid, string viewerID)
        {
            if (AppConfig.UseWebRtc)
            {
                return ViewerHubContext.Clients.Client(viewerID).SendAsync("ReceiveIceCandidate", candidate, sdpMlineIndex, sdpMid);
            }

            return Task.CompletedTask;
        }

        public Task SendMachineName(string machineName, string viewerID)
        {
            return ViewerHubContext.Clients.Client(viewerID).SendAsync("ReceiveMachineName", machineName);
        }

        public Task SendRtcOfferToBrowser(string sdp, string viewerID, IceServerModel[] iceServers)
        {
            if (AppConfig.UseWebRtc)
            {
                return ViewerHubContext.Clients.Client(viewerID).SendAsync("ReceiveRtcOffer", sdp, iceServers);
            }

            return Task.CompletedTask;
        }

        public Task SendScreenCapture(byte[] captureBytes, string viewerHubConnectionID, int left, int top, int width, int height, long imageQuality, bool endOfFrame)
        {
            return ViewerHubContext.Clients.Client(viewerHubConnectionID).SendAsync("ScreenCapture", captureBytes, left, top, width, height, imageQuality, endOfFrame);
        }

        public Task SendScreenDataToBrowser(string selectedDisplay, string[] displayNames, string browserHubConnectionId)
        {
            lock (ViewerList)
            {
                ViewerList.Add(browserHubConnectionId);
            }
            return ViewerHubContext.Clients.Client(browserHubConnectionId).SendAsync("ScreenData", selectedDisplay, displayNames);
        }

        public Task SendScreenSize(int width, int height, string viewerHubConnectionID)
        {
            return ViewerHubContext.Clients.Client(viewerHubConnectionID).SendAsync("ScreenSize", width, height);
        }

        public Task SendWindowsSessions(List<WindowsSession> windowsSessions, string viewerID)
        {
            return ViewerHubContext.Clients.Client(viewerID).SendAsync("WindowsSessions", windowsSessions);
        }
    }
}
