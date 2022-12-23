using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Server.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Ocsp;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;

namespace Remotely.Server.Services.RcImplementations
{
    public class ViewerPageDataProvider : IViewerPageDataProvider
    {
        private readonly IDataService _dataService;
        private readonly IApplicationConfig _appConfig;

        public ViewerPageDataProvider(IDataService dataService, IApplicationConfig appConfig)
        {
            _dataService = dataService;
            _appConfig = appConfig;
        }
    
        public string GetUserDisplayName(PageModel pageModel)
        {
            var user = _dataService.GetUserByNameWithOrg(pageModel.User.Identity.Name);
            return user.UserOptions?.DisplayName ?? user.UserName;
        }

        public ViewerPageTheme GetTheme(PageModel pageModel)
        {
            if (pageModel.User.Identity.IsAuthenticated)
            {
                var user = _dataService.GetUserByNameWithOrg(pageModel.User.Identity.Name);

                return user.UserOptions.Theme switch
                {
                    Theme.Light => ViewerPageTheme.Light,
                    Theme.Dark => ViewerPageTheme.Dark,
                    _ => ViewerPageTheme.Dark
                };
            }

            return _appConfig.Theme switch
            {
                Theme.Light => ViewerPageTheme.Light,
                Theme.Dark => ViewerPageTheme.Dark,
                _ => ViewerPageTheme.Dark
            };
        }
    }
}
