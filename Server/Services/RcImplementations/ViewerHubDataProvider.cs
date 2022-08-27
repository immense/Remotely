using Immense.RemoteControl.Server.Abstractions;
using Remotely.Server.Auth;

namespace Remotely.Server.Services.RcImplementations
{
    public class ViewerHubDataProvider : IViewerHubDataProvider
    {
        private readonly IDataService _dataService;
        private readonly IOtpProvider _otpProvider;
        private readonly IApplicationConfig _appConfig;

        public ViewerHubDataProvider(
            IDataService dataService,
            IOtpProvider otpProvider,
            IApplicationConfig appConfig)
        {
            _dataService = dataService;
            _otpProvider = otpProvider;
            _appConfig = appConfig;
        }

        public bool EnforceAttendedAccess => _appConfig.EnforceAttendedAccess;

        public bool RemoteControlNotifyUser => _appConfig.RemoteControlNotifyUser;

        public int RemoteControlSessionLimit => _appConfig.RemoteControlSessionLimit;

        public bool DoesUserHaveAccessToDevice(string targetDeviceId, string userIdentifier)
        {
            var user = _dataService.GetUserByID(userIdentifier);
            return _dataService.DoesUserHaveAccessToDevice(targetDeviceId, user);
        }

   
        public string GetOrganizationNameById(string orgId)
        {
            return _dataService.GetOrganizationNameById(orgId);
        }

        public string GetRequesterDisplayName(string userIdentifier)
        {
            var user = _dataService.GetUserByID(userIdentifier);
            if (!string.IsNullOrWhiteSpace(user.UserOptions?.DisplayName))
            {
                return user.UserOptions.DisplayName;
            }
            return user.UserName;
        }

        public string GetRequesterOrganizationId(string userIdentifier)
        {
            var user = _dataService.GetUserByID(userIdentifier);
            return user.OrganizationID;
        }

        public bool OtpMatchesDevice(string otp, string targetDeviceId)
        {
            return _otpProvider.OtpMatchesDevice(otp, targetDeviceId);
        }
    }
}
