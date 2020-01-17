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
        public static ConcurrentDictionary<string, RemotelyUser> OrganizationConnectionList { get; } = new ConcurrentDictionary<string, RemotelyUser>();
        private ApplicationConfig AppConfig { get; set; }
        private DataService DataService { get; }

        private IHubContext<DeviceSocketHub> DeviceHub { get; }

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

        private IHubContext<RCDeviceSocketHub> RCDeviceHub { get; }

        private RemoteControlSessionRecorder RCSessionRecorder { get; }

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
        public async Task CtrlAltDel()
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("CtrlAltDel", Context.ConnectionId);
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

        public override async Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                var user = DataService.GetUserByName(Context.User.Identity.Name);
                OrganizationConnectionList.AddOrUpdate(Context.ConnectionId, user, (id, r) => user);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                OrganizationConnectionList.Remove(Context.ConnectionId, out _);
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

        public async Task SendClipboardTransfer(string transferText, bool typeText)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("ClipboardTransfer", transferText, typeText, Context.ConnectionId);
        }

        public async Task SendLatencyUpdate(DateTime sentTime, int bytesRecieved)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("LatencyUpdate", sentTime, bytesRecieved, Context.ConnectionId);
        }

        public async Task SendQualityChange(int qualityLevel)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("QualityChange", qualityLevel, Context.ConnectionId);
        }

        public async Task SendAutoQualityAdjust(bool isOn)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("AutoQualityAdjust", isOn, Context.ConnectionId);
        }

        public async Task SendScreenCastRequestToDevice(string screenCasterID, string requesterName, int remoteControlMode)
        {
            if ((RemoteControlMode)remoteControlMode == RemoteControlMode.Normal)
            {
                if (!RCDeviceSocketHub.SessionInfoList.Any(x => x.Value.AttendedSessionID == screenCasterID))
                {
                    await Clients.Caller.SendAsync("SessionIDNotFound");
                    return;
                }

                screenCasterID = RCDeviceSocketHub.SessionInfoList.First(x => x.Value.AttendedSessionID == screenCasterID).Value.RCSocketID;
            }

            string orgId = null;

            if (Context?.User?.Identity?.IsAuthenticated == true)
            {
                orgId = DataService.GetUserByID(Context.UserIdentifier).OrganizationID;
            }

            RCDeviceSocketHub.SessionInfoList.TryGetValue(screenCasterID, out var sessionInfo);

            DataService.WriteEvent(new EventLog()
            {
                EventType = EventType.Info,
                TimeStamp = DateTime.Now,
                Message = $"Remote control session requested.  " +
                                $"Login ID (if logged in): {Context?.User?.Identity?.Name}.  " +
                                $"Machine Name: {sessionInfo.MachineName}.  " +
                                $"Requester Name (if specified): {requesterName}.  " +
                                $"Connection ID: {Context.ConnectionId}. User ID: {Context.UserIdentifier}.  " +
                                $"Screen Caster ID: {screenCasterID}.  " + 
                                $"Mode: {((RemoteControlMode)remoteControlMode).ToString()}.  " + 
                                $"Requester IP Address: " + Context?.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString(),
                OrganizationID = orgId
            });
    
            ScreenCasterID = screenCasterID;
            Mode = (RemoteControlMode)remoteControlMode;
            RequesterName = requesterName;

            if (Mode == RemoteControlMode.Unattended)
            {
                sessionInfo.Mode = RemoteControlMode.Unattended;
                var deviceID = DeviceSocketHub.ServiceConnections[sessionInfo.ServiceID].ID;
                if (Context.User.Identity.IsAuthenticated && DataService.DoesUserHaveAccessToDevice(deviceID, Context.UserIdentifier))
                {
                    await RCDeviceHub.Clients.Client(screenCasterID).SendAsync("GetScreenCast", Context.ConnectionId, requesterName);
                }
            }
            else
            {
                sessionInfo.Mode = RemoteControlMode.Normal;
                await Clients.Caller.SendAsync("RequestingScreenCast");
                await RCDeviceHub.Clients.Client(screenCasterID).SendAsync("RequestScreenCast", Context.ConnectionId, requesterName);
            }
        }
        public async Task SendSharedFileIDs(List<string> fileIDs)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("SharedFileIDs", fileIDs);
        }
        public async Task SendToggleAudio(bool toggleOn)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("ToggleAudio", toggleOn, Context.ConnectionId);
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

        public async Task SendIceCandidateToAgent(string candidate, int sdpMlineIndex, string sdpMid)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("ReceiveIceCandidate", candidate, sdpMlineIndex, sdpMid, Context.ConnectionId);
        }

        public async Task SendRtcAnswerToAgent(string sdp)
        {
            await RCDeviceHub.Clients.Client(ScreenCasterID).SendAsync("ReceiveRtcAnswer", sdp, Context.ConnectionId);
        }
    }
}
