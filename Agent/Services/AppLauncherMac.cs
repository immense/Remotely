using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Agent.Interfaces;

namespace Remotely.Agent.Services
{
    public class AppLauncherMac : IAppLauncher
    {
        public async Task<int> LaunchChatService(string orgName, string requesterID, HubConnection hubConnection)
        {
            await hubConnection.SendAsync("DisplayMessage", "Feature under development.", "Feature is under development.", "bg-warning", requesterID);
            return 0;
        }

        public async Task LaunchRemoteControl(int targetSessionId, string requesterID, string serviceID, HubConnection hubConnection)
        {
            await hubConnection.SendAsync("DisplayMessage", "Feature under development.", "Feature is under development.", "bg-warning", requesterID);
        }

        public async Task RestartScreenCaster(List<string> viewerIDs, string serviceID, string requesterID, HubConnection hubConnection, int targetSessionID = -1)
        {
            await hubConnection.SendAsync("DisplayMessage", "Feature under development.", "Feature is under development.", "bg-warning", requesterID);
        }
    }
}
