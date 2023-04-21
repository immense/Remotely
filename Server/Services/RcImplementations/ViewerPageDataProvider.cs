using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Server.Areas.RemoteControl.Pages;
using Immense.RemoteControl.Server.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Ocsp;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Threading.Tasks;

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
    
        public Task<string> GetUserDisplayName(PageModel pageModel)
        {
            if (string.IsNullOrWhiteSpace(pageModel?.User?.Identity?.Name))
            {
                return Task.FromResult(string.Empty);
            }

            var user = _dataService.GetUserByNameWithOrg(pageModel.User.Identity.Name);

            if (user is null)
            {
                return Task.FromResult(string.Empty);
            }

            var displayName = user.UserOptions?.DisplayName ?? user.UserName ?? string.Empty;
            return Task.FromResult(displayName);
        }

        public Task<ViewerPageTheme> GetTheme(PageModel pageModel)
        {
            if (pageModel.User.Identity.IsAuthenticated)
            {
                var user = _dataService.GetUserByNameWithOrg(pageModel.User.Identity.Name);

                var userTheme = user.UserOptions.Theme switch
                {
                    Theme.Light => ViewerPageTheme.Light,
                    Theme.Dark => ViewerPageTheme.Dark,
                    _ => ViewerPageTheme.Dark
                };
                return Task.FromResult(userTheme);
            }

            var appTheme = _appConfig.Theme switch
            {
                Theme.Light => ViewerPageTheme.Light,
                Theme.Dark => ViewerPageTheme.Dark,
                _ => ViewerPageTheme.Dark
            };
            return Task.FromResult(appTheme);
        }

        public Task<string> GetPageTitle(PageModel pageModel)
        {
            return Task.FromResult("Remotely Remote Control");
        }

        public Task<string> GetProductName(PageModel pageModel)
        {
            return Task.FromResult("Remotely");
        }

        public Task<string> GetProductSubtitle(PageModel pageModel)
        {
            return Task.FromResult("Remote Control");
        }

        public Task<string> GetPageDescription(ViewerModel viewerModel)
        {
            return Task.FromResult("Open-source remote support tools.");
        }
    }
}
