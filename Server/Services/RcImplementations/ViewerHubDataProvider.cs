using Immense.RemoteControl.Server.Abstractions;

namespace Remotely.Server.Services.RcImplementations
{
    public class ViewerHubDataProvider : IViewerHubDataProvider
    {
        public bool EnforceAttendedAccess => throw new System.NotImplementedException();

        public bool RemoteControlNotifyUser => throw new System.NotImplementedException();

        public bool DoesUserHaveAccessToDevice(string targetDeviceId, string userIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public int GetConcurrentSessionLimit()
        {
            throw new System.NotImplementedException();
        }

        public string GetOrganizationNameById(string orgId)
        {
            throw new System.NotImplementedException();
        }

        public string GetRequesterDisplayName(string userIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public string GetRequesterOrganizationId(string userIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public bool OtpMatchesDevice(string otp, string targetDeviceId)
        {
            throw new System.NotImplementedException();
        }
    }
}
