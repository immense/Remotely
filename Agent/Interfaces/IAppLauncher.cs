using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Remotely.Agent.Interfaces
{
    public interface IAppLauncher
    {
        Task<int> LaunchChatService(string orgName, string requesterID, HubConnection hubConnection);
        Task LaunchRemoteControl(int targetSessionId, string requesterID, string serviceID, HubConnection hubConnection);
        Task RestartScreenCaster(List<string> viewerIDs, string serviceID, string requesterID, HubConnection hubConnection, int targetSessionID = -1);
    }
}
