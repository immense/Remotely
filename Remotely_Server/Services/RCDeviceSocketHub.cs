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
            IHubContext<DeviceSocketHub> deviceSocketHub,
            ApplicationConfig appConfig,
            RandomGenerator rng)
        {
            DataService = dataService;
            BrowserHub = browserHub;
            RCBrowserHub = rcBrowserHub;
            AppConfig = appConfig;
            DeviceHub = deviceSocketHub;
            RNG = rng;
        }
        private ApplicationConfig AppConfig { get; set; }
        private IHubContext<DeviceSocketHub> DeviceHub { get; }
        private DataService DataService { get; }
        private IHubContext<BrowserSocketHub> BrowserHub { get; }
        private IHubContext<RCBrowserSocketHub> RCBrowserHub { get; }
        
        private RandomGenerator RNG { get; }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            if (AttendedSessionList.ContainsKey(Context.Items["SessionID"].ToString())) 
            {
                while (!AttendedSessionList.TryRemove(Context.Items["SessionID"].ToString(), out var value))
                {
                    await Task.Delay(1000);
                }
            }
        }

        public async Task SendScreenCountToBrowser(int primaryScreenIndex, int screenCount, string rcBrowserHubConnectionID)
        {
            await RCBrowserHub.Clients.Client(rcBrowserHubConnectionID).SendAsync("ScreenCount", primaryScreenIndex, screenCount);
        }
        public async Task SendScreenSize(int width, int height, string rcBrowserHubConnectionID)
        {
            await RCBrowserHub.Clients.Client(rcBrowserHubConnectionID).SendAsync("ScreenSize", width, height);
        }
        public async Task SendScreenCapture(byte[] captureBytes, string rcBrowserHubConnectionID)
        {
            await RCBrowserHub.Clients.Client(rcBrowserHubConnectionID).SendAsync("ScreenCapture", captureBytes, rcBrowserHubConnectionID);
        }
        public async Task NotifyRequesterUnattendedReady(string browserHubConnectionID)
        {
            await BrowserHub.Clients.Client(browserHubConnectionID).SendAsync("UnattendedSessionReady", Context.ConnectionId);
        }
        public async Task SendConnectionFailedToBrowser(string browserHubConnectionID)
        {
            await RCBrowserHub.Clients.Client(browserHubConnectionID).SendAsync("ConnectionFailed");
        }



        public async Task SendRTCSessionToBrowser(object offer, string browserHubConnectionID)
        {
            await RCBrowserHub.Clients.Client(browserHubConnectionID).SendAsync("RTCSession", offer);
        }
        public async Task SendIceCandidateToBrowser(object candidate, string browserHubConnectionID)
        {
            await RCBrowserHub.Clients.Client(browserHubConnectionID).SendAsync("IceCandidate", candidate);
        }
        

        public async Task NotifyViewerDesktopSwitching(string viewerID)
        {
            await RCBrowserHub.Clients.Client(viewerID).SendAsync("DesktopSwitching");
        }
        public async Task LaunchRCInNewDesktop(string serviceID, string[] viewerIDs, string desktop)
        {
            await DeviceHub.Clients.Client(serviceID).SendAsync("LaunchRCInNewDesktop", viewerIDs, serviceID, desktop);
        }
        public async Task NotifyRequesterDesktopSwitchCompleted(string rcBrowserConnectionID)
        {
            await RCBrowserHub.Clients.Client(rcBrowserConnectionID).SendAsync("SwitchedDesktop", Context.ConnectionId);
        }
        public async Task DesktopSwitchFailed(string rcBrowserConnectionID)
        {
            await RCBrowserHub.Clients.Client(rcBrowserConnectionID).SendAsync("DesktopSwitchFailed");
        }
        public async Task GetIceConfiguration()
        {
            //await Clients.Caller.SendAsync("IceConfiguration", AppConfig.IceConfiguration);
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
