using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Remotely.Server.Auth;
using Remotely.Server.Models;
using Remotely.Server.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Hubs
{
    [ServiceFilter(typeof(RemoteControlFilterAttribute))]
    public class ViewerHub : Hub
    {
        public ViewerHub(IDataService dataService,
            IHubContext<CasterHub> casterHubContext,
            IHubContext<AgentHub> agentHub,
            IApplicationConfig appConfig)
        {
            DataService = dataService;
            CasterHubContext = casterHubContext;
            AgentHubContext = agentHub;
            AppConfig = appConfig;
        }
        private IApplicationConfig AppConfig { get; set; }
        private IDataService DataService { get; }

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

        public Task SendDtoToClient(byte[] baseDto)
        {
            if (string.IsNullOrWhiteSpace(ScreenCasterID))
            {
                return Task.CompletedTask;
            }

            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("SendDtoToClient", baseDto, Context.ConnectionId);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (!string.IsNullOrWhiteSpace(ScreenCasterID))
            {
                CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("ViewerDisconnected", Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task SendIceCandidateToAgent(string candidate, int sdpMlineIndex, string sdpMid)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("ReceiveIceCandidate", candidate, sdpMlineIndex, sdpMid, Context.ConnectionId);
        }

        public Task SendRtcAnswerToAgent(string sdp)
        {
            return CasterHubContext.Clients.Client(ScreenCasterID).SendAsync("ReceiveRtcAnswer", sdp, Context.ConnectionId);
        }

        public async Task SendScreenCastRequestToDevice(string screenCasterID, string requesterName, int remoteControlMode, string otp)
        {
            if (string.IsNullOrWhiteSpace(screenCasterID))
            {
                return;
            }

            if ((RemoteControlMode)remoteControlMode == RemoteControlMode.Normal)
            {
                if (!CasterHub.SessionInfoList.Any(x => x.Value.AttendedSessionID == screenCasterID))
                {
                    await Clients.Caller.SendAsync("SessionIDNotFound");
                    return;
                }

                screenCasterID = CasterHub.SessionInfoList.First(x => x.Value.AttendedSessionID == screenCasterID).Value.CasterSocketID;
            }

            if (!CasterHub.SessionInfoList.TryGetValue(screenCasterID, out var sessionInfo))
            {
                await Clients.Caller.SendAsync("SessionIDNotFound");
                return;
            }

            SessionInfo = sessionInfo;
            ScreenCasterID = screenCasterID;
            RequesterName = requesterName;
            Mode = (RemoteControlMode)remoteControlMode;

            string orgId = null;

            if (Context?.User?.Identity?.IsAuthenticated == true)
            {
                var user = DataService.GetUserByID(Context.UserIdentifier);
                if (string.IsNullOrWhiteSpace(RequesterName))
                {
                    RequesterName = user.UserOptions.DisplayName ?? user.UserName;
                }
                orgId = user.OrganizationID;
                var currentUsers = CasterHub.SessionInfoList.Count(x =>
                    x.Key != screenCasterID &&
                    x.Value.OrganizationID == orgId);
                if (currentUsers >= AppConfig.RemoteControlSessionLimit)
                {
                    await Clients.Caller.SendAsync("ShowMessage", "Max number of concurrent sessions reached.");
                    Context.Abort();
                    return;
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
                                $"Requester Name (if specified): {RequesterName}.  " +
                                $"Connection ID: {Context.ConnectionId}. User ID: {Context.UserIdentifier}.  " +
                                $"Screen Caster ID: {screenCasterID}.  " +
                                $"Mode: {(RemoteControlMode)remoteControlMode}.  " +
                                $"Requester IP Address: " + Context?.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString(),
                OrganizationID = orgId
            });


            if (Mode == RemoteControlMode.Unattended)
            {
                var targetDevice = AgentHub.ServiceConnections[SessionInfo.ServiceID];

                var useWebRtc = targetDevice.WebRtcSetting == WebRtcSetting.Default ?
                            AppConfig.UseWebRtc :
                            targetDevice.WebRtcSetting == WebRtcSetting.Enabled;


                SessionInfo.Mode = RemoteControlMode.Unattended;

                if ((!string.IsNullOrWhiteSpace(otp) &&
                        RemoteControlFilterAttribute.OtpMatchesDevice(otp, targetDevice.ID))
                    ||
                    (Context.User.Identity.IsAuthenticated &&
                        DataService.DoesUserHaveAccessToDevice(targetDevice.ID, Context.UserIdentifier)))
                {
                    var orgName = DataService.GetOrganizationNameById(orgId);
                    await CasterHubContext.Clients.Client(screenCasterID).SendAsync("GetScreenCast", 
                        Context.ConnectionId, 
                        RequesterName, 
                        AppConfig.RemoteControlNotifyUser,
                        AppConfig.EnforceAttendedAccess,
                        useWebRtc,
                        orgName);
                }
                else
                {
                    await Clients.Caller.SendAsync("Unauthorized");
                }
            }
            else
            {
                SessionInfo.Mode = RemoteControlMode.Normal;
                await Clients.Caller.SendAsync("RequestingScreenCast");
                await CasterHubContext.Clients.Client(screenCasterID).SendAsync("RequestScreenCast", Context.ConnectionId, RequesterName, AppConfig.RemoteControlNotifyUser, AppConfig.UseWebRtc);
            }
        }

    }
}
