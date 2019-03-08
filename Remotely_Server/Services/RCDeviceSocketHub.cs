using Remotely_Library.Models;
using Remotely_Library.Services;
using Remotely_Server.Data;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely_Server.Services
{
    public class RCDeviceSocketHub : Hub
    {
        public static ConcurrentDictionary<string, string> AttendedSessionList { get; set; } = new ConcurrentDictionary<string, string>();
        public RCDeviceSocketHub(DataService dataService, 
            IHubContext<BrowserSocketHub> browserHub, 
            IHubContext<RCBrowserSocketHub> rcBrowserHub, 
            IHubContext<DeviceSocketHub> deviceSocketHub)
        {
            DataService = dataService;
            BrowserHub = browserHub;
            RCBrowserHub = rcBrowserHub;
            DeviceHub = deviceSocketHub;
        }
        private IHubContext<DeviceSocketHub> DeviceHub { get; }
        private DataService DataService { get; }
        private IHubContext<BrowserSocketHub> BrowserHub { get; }
        private IHubContext<RCBrowserSocketHub> RCBrowserHub { get; }
        private string ServiceID
        {
            get
            {
                if (Context.Items.ContainsKey("ServiceID"))
                {
                    return Context.Items["ServiceID"] as string;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Context.Items["ServiceID"] = value;
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

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {           
            if (Context.Items.ContainsKey("SessionID") && AttendedSessionList.ContainsKey(Context.Items["SessionID"].ToString())) 
            {
                while (!AttendedSessionList.TryRemove(Context.Items["SessionID"].ToString(), out var value))
                {
                    await Task.Delay(1000);
                }
                await RCBrowserHub.Clients.Clients(ViewerList).SendAsync("ScreenCasterDisconnected");
            }
            else
            {
                if (ViewerList.Count > 0)
                {
                    await RCBrowserHub.Clients.Clients(ViewerList).SendAsync("Reconnecting");
                    await DeviceHub.Clients.Client(ServiceID).SendAsync("RestartScreenCaster", ViewerList, ServiceID, Context.ConnectionId);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
        public void ReceiveServiceID(string serviceID)
        {
            ServiceID = serviceID;
        }
        public void ViewerDisconnected(string viewerID)
        {
            lock (ViewerList)
            {
                ViewerList.Remove(viewerID);
            }
        }
        public async Task SendScreenCountToBrowser(int primaryScreenIndex, int screenCount, string rcBrowserHubConnectionID)
        {
            lock (ViewerList)
            {
                ViewerList.Add(rcBrowserHubConnectionID);
            }
            await RCBrowserHub.Clients.Client(rcBrowserHubConnectionID).SendAsync("ScreenCount", primaryScreenIndex, screenCount);
        }

        public async Task SendScreenSize(int width, int height, string rcBrowserHubConnectionID)
        {
            await RCBrowserHub.Clients.Client(rcBrowserHubConnectionID).SendAsync("ScreenSize", width, height);
        }

        public Task SendScreenCapture(byte[] captureBytes, string rcBrowserHubConnectionID, DateTime captureTime)
        {
            return RCBrowserHub.Clients.Client(rcBrowserHubConnectionID).SendAsync("ScreenCapture", captureBytes, captureTime);
        }

        public async Task NotifyRequesterUnattendedReady(string browserHubConnectionID)
        {
            await BrowserHub.Clients.Client(browserHubConnectionID).SendAsync("UnattendedSessionReady", Context.ConnectionId);
        }
        public async Task NotifyViewersRelaunchedScreenCasterReady(string[] viewerIDs)
        {
            await RCBrowserHub.Clients.Clients(viewerIDs).SendAsync("RelaunchedScreenCasterReady", Context.ConnectionId);
        }
        public async Task SendConnectionFailedToViewers(List<string> viewerIDs)
        {
            await RCBrowserHub.Clients.Clients(viewerIDs).SendAsync("ConnectionFailed");
        }

        public async Task SendCursorChange(string cursor, List<string> viewerIDs)
        {
            await RCBrowserHub.Clients.Clients(viewerIDs).SendAsync("CursorChange", cursor);
        }

        public async Task SwitchingDesktops(string[] viewerIDs)
        {
            await RCBrowserHub.Clients.Clients(viewerIDs).SendAsync("SwitchingDesktops");
        }

        public async Task GetSessionID()
        {
            var random = new Random();
            var sessionID = "";
            for (var i = 0; i < 3; i++)
            {
                sessionID += random.Next(0, 999).ToString().PadLeft(3, '0');
            }
            Context.Items["SessionID"] = sessionID;

            while (!AttendedSessionList.TryAdd(sessionID, Context.ConnectionId))
            {
                await Task.Delay(1000);
            }

            await Clients.Caller.SendAsync("SessionID", sessionID);
        }
    }
}
