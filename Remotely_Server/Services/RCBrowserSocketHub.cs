using Remotely_Library.Models;
using Remotely_Server.Data;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely_Server.Services
{
    public class RCBrowserSocketHub : Hub
    {
        public RCBrowserSocketHub(DataService dataService, IHubContext<RCDeviceSocketHub> rcDeviceHub, IHubContext<DeviceSocketHub> deviceHub, ApplicationConfig appConfig)
        {
            this.DataService = dataService;
            this.RCDeviceHub = rcDeviceHub;
            this.AppConfig = appConfig;
            this.DeviceHub = deviceHub;
        }
        public static ConcurrentDictionary<string, RemotelyUser> OrganizationConnectionList { get; set; } = new ConcurrentDictionary<string, RemotelyUser>();
        private DataService DataService { get; }
        private IHubContext<RCDeviceSocketHub> RCDeviceHub { get; }

        private ApplicationConfig AppConfig { get; set; }
        private IHubContext<DeviceSocketHub> DeviceHub { get; }

        private string ClientID
        {
            get
            {
                return Context.Items["ClientID"] as string;
            }
        }
        private string ClientType
        {
            get
            {
                return Context.Items["ClientType"] as string;
            }
        }
        private string RequesterName
        {
            get
            {
                return Context.Items["RequesterName"] as string;
            }
        }
        public async Task GetIceConfiguration()
        {
            //await Clients.Caller.SendAsync("IceConfiguration", AppConfig.IceConfiguration);
        }
        public async Task SendRTCSessionToDevice(object offer, string clientType, string clientID)
        {
            if (clientType == "Normal")
            {
                clientID = RCDeviceSocketHub.AttendedSessionList[clientID];
            }
            await RCDeviceHub.Clients.Client(clientID).SendAsync("RTCSession", offer, Context.ConnectionId);
        }
        public async Task SendIceCandidateToDevice(object candidate, string clientType, string clientID)
        {
            if (clientType == "Normal")
            {
                clientID = RCDeviceSocketHub.AttendedSessionList[clientID];
            }
            await RCDeviceHub.Clients.Client(clientID).SendAsync("IceCandidate", candidate, Context.ConnectionId);
        }
        public async Task SendOfferRequestToDevice(string clientID, string requesterName, string clientType)
        {
            if (clientType == "Normal")
            {
                if (!RCDeviceSocketHub.AttendedSessionList.ContainsKey(clientID))
                {
                    await Clients.Caller.SendAsync("SessionIDNotFound");
                    return;
                }

                clientID = RCDeviceSocketHub.AttendedSessionList[clientID];
            }
            DataService.WriteEvent(new EventLog()
            {
                EventType = EventTypes.Info,
                TimeStamp = DateTime.Now,
                Message = $"Remote control session requested by {requesterName}.  " +
                                $"Connection ID: {Context.ConnectionId}. User ID: {Context.UserIdentifier}.  " +
                                $"Login ID (if logged in): {Context?.User?.Identity?.Name}.  " +
                                $"Requester IP Address: " + Context?.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString()
            });
            Context.Items["ClientID"] = clientID;
            Context.Items["ClientType"] = clientType;
            Context.Items["RequesterName"] = requesterName;
            await RCDeviceHub.Clients.Client(clientID).SendAsync("GetOfferRequest", Context.ConnectionId, requesterName);
        }
        public async Task SelectScreen(int screenIndex)
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("SelectScreen", screenIndex, Context.ConnectionId);
        }
        public async Task MouseMove(double percentX, double percentY)
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("MouseMove", percentX, percentY, Context.ConnectionId);
        }
        public async Task MouseDown(string button, double percentX, double percentY)
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("MouseDown", button, percentX, percentY, Context.ConnectionId);
        }
        public async Task MouseUp(string button, double percentX, double percentY)
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("MouseUp", button, percentX, percentY, Context.ConnectionId);
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
        }
        public async Task TouchDown()
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("TouchDown", Context.ConnectionId);
        }
        public async Task LongPress()
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("LongPress", Context.ConnectionId);
        }
        public async Task TouchMove(double moveX, double moveY)
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("TouchMove", moveX, moveY, Context.ConnectionId);
        }
        public async Task TouchUp()
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("TouchUp", Context.ConnectionId);
        }
        public async Task Tap()
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("Tap", Context.ConnectionId);
        }
        public async Task MouseWheel(double deltaX, double deltaY)
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("MouseWheel", deltaX, deltaY, Context.ConnectionId);
        }
        public async Task KeyDown(string key)
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("KeyDown", key, Context.ConnectionId);
        }
        public async Task KeyUp(string key)
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("KeyUp", key, Context.ConnectionId);
        }
        public async Task KeyPress(string key)
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("KeyPress", key, Context.ConnectionId);
        }
        public async Task CtrlAltDel(string serviceID)
        {
            await DeviceHub.Clients.Client(serviceID).SendAsync("CtrlAltDel");
        }
        public async Task SendSharedFileIDs(List<string> fileIDs)
        {
            await RCDeviceHub.Clients.Client(ClientID).SendAsync("SharedFileIDs", fileIDs, Context.ConnectionId);
        }
    }
}
