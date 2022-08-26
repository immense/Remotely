using Immense.RemoteControl.Server.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Remotely.Server.Services.RcImplementations
{
    public class HubEventHandler : IHubEventHandler
    {
        public Task ChangeWindowsSession(string serviceConnectionId, string viewerConnectionId, int targetWindowsSession)
        {
            throw new System.NotImplementedException();
        }

        public void LogRemoteControlStarted(string message, string organizationId)
        {
            throw new System.NotImplementedException();
        }

        public Task NotifyUnattendedSessionReady(string userConnectionId, string desktopConnectionId, string deviceId)
        {
            throw new System.NotImplementedException();
        }

        public Task RestartScreenCaster(string desktopConnectionId, string serviceConnectionId, HashSet<string> viewerList)
        {
            throw new System.NotImplementedException();
        }
    }
}
