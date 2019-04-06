using Remotely_Server.Data;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Drawing;
using Remotely_Shared.Models;

namespace Remotely_Server.Services
{
    public class RCDeviceSocketHub : Hub
    {
        public static ConcurrentDictionary<string, string> AttendedSessionList { get; set; } = new ConcurrentDictionary<string, string>();

        public static ConcurrentDictionary<string, string> MachineNameToSessionIDLookup { get; set; } = new ConcurrentDictionary<string, string>();

        public RCDeviceSocketHub(DataService dataService, 
            IHubContext<BrowserSocketHub> browserHub, 
            IHubContext<RCBrowserSocketHub> rcBrowserHub, 
            IHubContext<DeviceSocketHub> deviceSocketHub,
            RemoteControlSessionRecorder rcSessionRecorder,
            ApplicationConfig appConfig)
        {
            DataService = dataService;
            BrowserHub = browserHub;
            RCBrowserHub = rcBrowserHub;
            DeviceHub = deviceSocketHub;
            RCSessionRecorder = rcSessionRecorder;
            AppConfig = appConfig;
        }
        private IHubContext<DeviceSocketHub> DeviceHub { get; }
        public RemoteControlSessionRecorder RCSessionRecorder { get; }
        public ApplicationConfig AppConfig { get; }
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
        private string MachineName
        {
            get
            {
                if (Context.Items.ContainsKey("MachineName"))
                {
                    return Context.Items["MachineName"] as string;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Context.Items["MachineName"] = value;
            }
        }
        private Size CurrentScreenSize
        {
            get
            {
                if (Context.Items.ContainsKey("CurrentScreenSize"))
                {
                    return (Size)Context.Items["CurrentScreenSize"];
                }
                else
                {
                    return Size.Empty;
                }
            }
            set
            {
                Context.Items["CurrentScreenSize"] = value;
            }
        }

        private DateTime StartTime
        {
            get
            {
                if (Context.Items.ContainsKey("StartTime"))
                {
                    return (DateTime)Context.Items["StartTime"];
                }
                else
                {
                    return DateTime.Now;
                }
            }
            set
            {
                Context.Items["StartTime"] = value;
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
            StartTime = DateTime.Now;
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

            if (!string.IsNullOrWhiteSpace(MachineName) && MachineNameToSessionIDLookup.ContainsKey(MachineName))
            {
                while (!MachineNameToSessionIDLookup.TryRemove(MachineName, out _))
                {
                    await Task.Delay(1000);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
        public void ReceiveDeviceInfo(string serviceID, string machineName)
        {
            ServiceID = serviceID;
            MachineName = machineName;
            MachineNameToSessionIDLookup[MachineName] = Context.ConnectionId;
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
            CurrentScreenSize = new Size(width, height);
            await RCBrowserHub.Clients.Client(rcBrowserHubConnectionID).SendAsync("ScreenSize", width, height);
        }

        public Task SendScreenCapture(byte[] captureBytes, string rcBrowserHubConnectionID, int left, int top, int width, int height, DateTime captureTime)
        {
            if (AppConfig.RecordRemoteControlSessions)
            {
                RCSessionRecorder.SaveFrame(captureBytes, left, top, CurrentScreenSize.Width, CurrentScreenSize.Height, rcBrowserHubConnectionID, MachineName, StartTime);
            }

            return RCBrowserHub.Clients.Client(rcBrowserHubConnectionID).SendAsync("ScreenCapture", captureBytes, left, top, width, height, captureTime);
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

        public async Task SendCursorChange(CursorInfo cursor, List<string> viewerIDs)
        {
            await RCBrowserHub.Clients.Clients(viewerIDs).SendAsync("CursorChange", cursor);
        }

        public async Task SwitchingDesktops(string[] viewerIDs)
        {
            await RCBrowserHub.Clients.Clients(viewerIDs).SendAsync("SwitchingDesktops");
        }

        public async Task SendViewerRemoved(string viewerID)
        {
            await RCBrowserHub.Clients.Clients(viewerID).SendAsync("ViewerRemoved");
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
