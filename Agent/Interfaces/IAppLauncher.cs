using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Remotely.Agent.Interfaces;

public interface IAppLauncher
{
    Task<int> LaunchChatService(string pipeName, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection);
    Task LaunchRemoteControl(int targetSessionId, string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection);
    Task RestartScreenCaster(string[] viewerIds, string sessionId, string accessKey, string userConnectionId, string requesterName, string orgName, string orgId, HubConnection hubConnection, int targetSessionID = -1);
}
