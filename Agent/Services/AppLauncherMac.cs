using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Agent.Interfaces;

namespace Remotely.Agent.Services
{
    public class AppLauncherMac : IAppLauncher
    {

        public async Task<int> LaunchChatService(string pipeName, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection)
        {
            await hubConnection.SendAsync("DisplayMessage", "Feature under development.", "Feature is under development.", "bg-warning", userConnectionId);
            return 0;
        }

        public async Task LaunchRemoteControl(int targetSessionId, string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection)
        {
            await hubConnection.SendAsync("DisplayMessage", "Feature under development.", "Feature is under development.", "bg-warning", userConnectionId);
        }

        public async Task RestartScreenCaster(List<string> viewerIDs, string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection, int targetSessionID = -1)
        {
            await hubConnection.SendAsync("DisplayMessage", "Feature under development.", "Feature is under development.", "bg-warning", userConnectionId);
        }
    }
}
