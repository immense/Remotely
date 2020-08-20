using Remotely.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotely.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remotely.Server.Attributes;
using Remotely.Server.Models;
using Remotely.Server.Services;

namespace Remotely.Server.Hubs
{
    [ServiceFilter(typeof(RemoteControlFilterAttribute))]
    public class RCBrowserHub : Hub
    {
        public RCBrowserHub(DataService dataService,
            IHubContext<CasterHub> casterHubContext,
            IHubContext<AgentHub> agentHub,
            ApplicationConfig appConfig)
        {
            DataService = dataService;
            CasterHubContext = casterHubContext;
            AgentHubContext = agentHub;
            AppConfig = appConfig;
        }
        private ApplicationConfig AppConfig { get; set; }
        private DataService DataService { get; }

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

        private IHubContext<CasterHub> CasterHubContext { get; }
        private IHubContext<AgentHub> AgentHubContext { get; }
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
        public Task CtrlAltDel()
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("CtrlAltDel", Context.ConnectionId);
        }
        public Task GetWindowsSessions()
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("GetWindowsSessions", Context.ConnectionId);
        }
        public Task ChangeWindowsSession(int sessionID)
        {
            if (SessionInfo?.Mode == RemoteControlMode.Unattended)
            {
                return AgentHubContext.Clients
                    .Client(SessionInfo.ServiceID)
                    .SendAsync("ChangeWindowsSession", 
                        SessionInfo.ServiceID, 
                        Context.ConnectionId,
                        sessionID);
            }
            return Task.CompletedTask;
        }
        public Task KeyDown(string key)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("KeyDown", key, Context.ConnectionId);
        }

        public Task KeyPress(string key)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("KeyPress", key, Context.ConnectionId);
        }

        public Task KeyUp(string key)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("KeyUp", key, Context.ConnectionId);
        }

        public Task LongPress()
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("LongPress", Context.ConnectionId);
        }

        public Task MouseDown(int button, double percentX, double percentY)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("MouseDown", button, percentX, percentY, Context.ConnectionId);
        }

        public Task MouseMove(double percentX, double percentY)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("MouseMove", percentX, percentY, Context.ConnectionId);
        }

        public Task MouseUp(int button, double percentX, double percentY)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("MouseUp", button, percentX, percentY, Context.ConnectionId);
        }

        public Task MouseWheel(double deltaX, double deltaY)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("MouseWheel", deltaX, deltaY, Context.ConnectionId);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (ScreenCasterID != null)
            {
                CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("ViewerDisconnected", Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task SelectScreen(string displayName)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("SelectScreen", displayName, Context.ConnectionId);
        }

        public Task SendAutoQualityAdjust(bool isOn)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("AutoQualityAdjust", isOn, Context.ConnectionId);
        }

        public Task SendClipboardTransfer(string transferText, bool typeText)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("ClipboardTransfer", transferText, typeText, Context.ConnectionId);
        }
        public async Task SendFile(byte[] buffer, string fileName, string messageId, bool endOfFile, bool startOfFile)
        {
            await CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("ReceiveFile",
                buffer,
                fileName,
                messageId,
                endOfFile,
                startOfFile);
        }
        public Task SendFrameReceived()
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("FrameReceived", Context.ConnectionId);
        }
        public Task SendIceCandidateToAgent(string candidate, int sdpMlineIndex, string sdpMid)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("ReceiveIceCandidate", candidate, sdpMlineIndex, sdpMid, Context.ConnectionId);
        }

        public Task SendQualityChange(int qualityLevel)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("QualityChange", qualityLevel, Context.ConnectionId);
        }
        public Task SendRtcAnswerToAgent(string sdp)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("ReceiveRtcAnswer", sdp, Context.ConnectionId);
        }

        public async Task<Task> SendScreenCastRequestToDevice(string screenCasterID, string requesterName, int remoteControlMode, string otp)
        {
            if ((RemoteControlMode)remoteControlMode == RemoteControlMode.Normal)
            {
                if (!CasterHub.SessionInfoList.Any(x => x.Value.AttendedSessionID == screenCasterID))
                {
                    return Clients.Caller.SendAsync("SessionIDNotFound");
                }

                screenCasterID = CasterHub.SessionInfoList.First(x => x.Value.AttendedSessionID == screenCasterID).Value.CasterSocketID;
            }

            if (!CasterHub.SessionInfoList.TryGetValue(screenCasterID, out var sessionInfo))
            {
                return Clients.Caller.SendAsync("SessionIDNotFound");
            }

            SessionInfo = sessionInfo;
            ScreenCasterID = screenCasterID;
            RequesterName = requesterName;
            Mode = (RemoteControlMode)remoteControlMode;

            string orgId = null;

            if (Context?.User?.Identity?.IsAuthenticated == true)
            {
                orgId = DataService.GetUserByID(Context.UserIdentifier).OrganizationID;
                var currentUsers = CasterHub.SessionInfoList.Count(x => 
                    x.Key != screenCasterID &&
                    x.Value.OrganizationID == orgId);
                if (currentUsers >= AppConfig.RemoteControlSessionLimit)
                {
                    await Clients.Caller.SendAsync("ShowMessage", "Max number of concurrent sessions reached.");
                    Context.Abort();
                    return Task.CompletedTask;
                }
                SessionInfo.OrganizationID = orgId;
                SessionInfo.RequesterUserName = Context.User.Identity.Name;
                SessionInfo.RequesterSocketID = Context.ConnectionId;
            }

            DataService.WriteEvent(new EventLog()
            {
                EventType = EventType.Info,
                TimeStamp = DateTimeOffset.Now,
                Message = $"Remote control session requested.  " +
                                $"Login ID (if logged in): {Context?.User?.Identity?.Name}.  " +
                                $"Machine Name: {SessionInfo.MachineName}.  " +
                                $"Requester Name (if specified): {requesterName}.  " +
                                $"Connection ID: {Context.ConnectionId}. User ID: {Context.UserIdentifier}.  " +
                                $"Screen Caster ID: {screenCasterID}.  " + 
                                $"Mode: {(RemoteControlMode)remoteControlMode}.  " + 
                                $"Requester IP Address: " + Context?.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString(),
                OrganizationID = orgId
            });

            if (Mode == RemoteControlMode.Unattended)
            {
                SessionInfo.Mode = RemoteControlMode.Unattended;
                var deviceID = AgentHub.ServiceConnections[SessionInfo.ServiceID].ID;

                if ((!string.IsNullOrWhiteSpace(otp) &&
                        RemoteControlFilterAttribute.OtpMatchesDevice(otp, deviceID)) 
                    ||
                    (Context.User.Identity.IsAuthenticated &&
                        DataService.DoesUserHaveAccessToDevice(deviceID, Context.UserIdentifier)))
                {
                    return CasterHubContext.Clients.Client(screenCasterID).SendAsync("GetScreenCast", Context.ConnectionId, requesterName, AppConfig.RemoteControlNotifyUser);
                }
                else
                {
                    return Clients.Caller.SendAsync("Unauthorized");
                }
            }
            else
            {
                SessionInfo.Mode = RemoteControlMode.Normal;
                _ = Clients.Caller.SendAsync("RequestingScreenCast");
                return CasterHubContext.Clients.Client(screenCasterID).SendAsync("RequestScreenCast", Context.ConnectionId, requesterName, AppConfig.RemoteControlNotifyUser);
            }
        }
        public Task SendSetKeyStatesUp()
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("SetKeyStatesUp", Context.ConnectionId);
        }

        public Task SendSharedFileIDs(List<string> fileIDs)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("SharedFileIDs", fileIDs);
        }
        public Task SendToggleAudio(bool toggleOn)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("ToggleAudio", toggleOn, Context.ConnectionId);
        }
        public Task SendToggleBlockInput(bool toggleOn)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("ToggleBlockInput", toggleOn, Context.ConnectionId);
        }
        public Task SendToggleWebRtcVideo(bool toggleOn)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("ToggleWebRtcVideo", toggleOn, Context.ConnectionId);
        }

        public Task Tap(double percentX, double percentY)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("Tap", percentX, percentY, Context.ConnectionId);
        }

        public Task TouchDown()
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("TouchDown", Context.ConnectionId);
        }
        public Task TouchMove(double moveX, double moveY)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("TouchMove", moveX, moveY, Context.ConnectionId);
        }
        public Task TouchUp()
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("TouchUp", Context.ConnectionId);
        }
    }
}
