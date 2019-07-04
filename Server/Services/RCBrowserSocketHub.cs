using Remotely.Shared.Models;
using Remotely.Server.Data;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotely.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Remotely.Server.Services
{
    [Authorize("RemoteControlPolicy")]
    public class RCBrowserSocketHub : Hub
    {
        public RCBrowserSocketHub(DataService dataService, 
            IHubContext<RCDeviceSocketHub> rcDeviceHub, 
            IHubContext<DeviceSocketHub> deviceHub, 
            ApplicationConfig appConfig,
            RemoteControlSessionRecorder rcSessionRecorder)
        {
            this.DataService = dataService;
            this.RCDeviceHub = rcDeviceHub;
            this.AppConfig = appConfig;
            this.DeviceHub = deviceHub;
            RCSessionRecorder = rcSessionRecorder;
        }
        public static ConcurrentDictionary<string, RemotelyUser> OrganizationConnectionList { get; set; } = new ConcurrentDictionary<string, RemotelyUser>();
        private ApplicationConfig AppConfig { get; set; }
        private string ScreenCasterID
        {
            get
            {
                return Context.Items["ScreenCasterID"] as string;
            }
            set
            {
                Context.Items["ScreenCasterID"] = value;
            }
        }

        private RemoteControlMode Mode
        {
            get
            {
                return (RemoteControlMode)Context.Items["Mode"];
            }
            set
            {
                Context.Items["Mode"] = value;
            }
        }

        private DataService DataService { get; }
        private IHubContext<DeviceSocketHub> DeviceHub { get; }
        private RemoteControlSessionRecorder RCSessionRecorder { get; }
        private IHubContext<RCDeviceSocketHub> RCDeviceHub { get; }
        private string RequesterName
        {
            get
            {
                return Context.Items["RequesterName"] as string;
            }
            set
            {
                Context.Items["RequesterName"] = value;
            }
        }

        public async Task CtrlAltDel(string serviceID)
        {
            await DeviceHub.Clients.Client(serviceID).SendAsync("CtrlAltDel");
        }

        public async Task KeyDown(string key)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("KeyDown", key, Context.ConnectionId);
        }

        public async Task KeyPress(string key)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("KeyPress", key, Context.ConnectionId);
        }

        public async Task KeyUp(string key)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("KeyUp", key, Context.ConnectionId);
        }

        public async Task LongPress()
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("LongPress", Context.ConnectionId);
        }

        public async Task MouseDown(int button, double percentX, double percentY)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("MouseDown", button, percentX, percentY, Context.ConnectionId);
        }

        public async Task MouseMove(double percentX, double percentY)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("MouseMove", percentX, percentY, Context.ConnectionId);
        }

        public async Task MouseUp(int button, double percentX, double percentY)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("MouseUp", button, percentX, percentY, Context.ConnectionId);
        }

        public async Task MouseWheel(double deltaX, double deltaY)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("MouseWheel", deltaX, deltaY, Context.ConnectionId);
        }

        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                var user = DataService.GetUserByName(Context.User.Identity.Name);
                OrganizationConnectionList.TryAdd(Context.ConnectionId, user);
            }
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                while (!OrganizationConnectionList.TryRemove(Context.ConnectionId, out var user))
                {
                    await Task.Delay(1000);
                }
            }
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("ViewerDisconnected", Context.ConnectionId);

            if (AppConfig.RecordRemoteControlSessions)
            {
                RCSessionRecorder.StopProcessing(Context.ConnectionId);
            }
        }

        public async Task SelectScreen(int screenIndex)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("SelectScreen", screenIndex, Context.ConnectionId);
        }

        public async Task SendScreenCastRequestToDevice(string screenCasterID, string requesterName, int remoteControlMode)
        {
            if ((RemoteControlMode)remoteControlMode == RemoteControlMode.Normal)
            {
                if (!RCDeviceSocketHub.AttendedSessionList.ContainsKey(screenCasterID))
                {
                    await Clients.Caller.SendAsync("SessionIDNotFound");
                    return;
                }

                screenCasterID = RCDeviceSocketHub.AttendedSessionList[screenCasterID];
            }
           
            DataService.WriteEvent(new EventLog()
            {
                EventType = EventTypes.Info,
                TimeStamp = DateTime.Now,
                Message = $"Remote control session requested.  " +
                                $"Connection ID: {Context.ConnectionId}. User ID: {Context.UserIdentifier}.  " +
                                $"Screen Caster ID: {screenCasterID}.  " + 
                                $"Mode: {((RemoteControlMode)remoteControlMode).ToString()}.  " + 
                                $"Login ID (if logged in): {Context?.User?.Identity?.Name}.  " +
                                $"Rquester Name (if specified): {requesterName}.  " +
                                $"Requester IP Address: " + Context?.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString()
            });
    
            ScreenCasterID = screenCasterID;
            Mode = (RemoteControlMode)remoteControlMode;
            RequesterName = requesterName;
            if (Mode == RemoteControlMode.Unattended)
            {
                await RCDeviceHub.Clients.Client(screenCasterID).SendAsync("GetScreenCast", Context.ConnectionId, requesterName);
            }
            else
            {
                await Clients.Caller.SendAsync("RequestingScreenCast");
                await RCDeviceHub.Clients.Client(screenCasterID).SendAsync("RequestScreenCast", Context.ConnectionId, requesterName);
            }
        }

        public async Task SendLatencyUpdate(double latency)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("LatencyUpdate", latency, Context.ConnectionId);
        }
        public async Task SendSharedFileIDs(List<string> fileIDs)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("SharedFileIDs", fileIDs);
        }
        public async Task SendQualityChange(int qualityLevel)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("QualityChange", qualityLevel, Context.ConnectionId);
        }
        public async Task SendToggleAudio(bool toggleOn)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("ToggleAudio", toggleOn, Context.ConnectionId);
        }
        public async Task SendClipboardTransfer(string transferText)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("ClipboardTransfer", transferText, Context.ConnectionId);
        }
        public async Task Tap(double percentX, double percentY)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("Tap", percentX, percentY, Context.ConnectionId);
        }

        public async Task TouchDown()
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("TouchDown", Context.ConnectionId);
        }
        public async Task TouchMove(double moveX, double moveY)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("TouchMove", moveX, moveY, Context.ConnectionId);
        }
        public async Task TouchUp()
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("TouchUp", Context.ConnectionId);
        }
    }
}
