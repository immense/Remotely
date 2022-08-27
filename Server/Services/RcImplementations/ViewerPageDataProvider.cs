using Immense.RemoteControl.Server.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Ocsp;
using Remotely.Shared.Models;
using System;

namespace Remotely.Server.Services.RcImplementations
{
    public class ViewerPageDataProvider : IViewerPageDataProvider
    {
        private IDataService _dataService;
        private IApplicationConfig _appConfig;

        public ViewerPageDataProvider(IDataService dataService, IApplicationConfig appConfig)
        {
            _dataService = dataService;
            _appConfig = appConfig;
        }
        public string GetThemeUrl(PageModel pageModel)
        {
  
            if (pageModel.User.Identity.IsAuthenticated)
            {
                var user = _dataService.GetUserByNameWithOrg(pageModel.User.Identity.Name);

                switch (user.UserOptions.Theme)
                {
                    case Remotely.Shared.Enums.Theme.Light:
                        return "_content/Immense.RemoteControl.Server/css/remote-control-light.css";
                    case Remotely.Shared.Enums.Theme.Dark:
                        return "_content/Immense.RemoteControl.Server/css/remote-control-dark.css";
                    default:
                        break;
                }
            }

            if (_appConfig.Theme == Remotely.Shared.Enums.Theme.Light)
            {
                return "_content/Immense.RemoteControl.Server/css/remote-control-light.css";
            }
            else
            {
                return "_content/Immense.RemoteControl.Server/css/remote-control-dark.css";
            }
        }

        public string GetUserDisplayName(PageModel pageModel)
        {
            var user = _dataService.GetUserByNameWithOrg(pageModel.User.Identity.Name);
            return user.UserOptions?.DisplayName ?? user.UserName;
        }
    }
}
