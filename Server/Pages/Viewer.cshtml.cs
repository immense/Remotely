using Remotely.Server.Abstractions;
using Remotely.Server.Filters;
using Remotely.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Remotely.Server.Areas.RemoteControl.Pages;

[ServiceFilter(typeof(ViewerAuthorizationFilter))]
public class ViewerModel : PageModel
{
    private readonly IViewerPageDataProvider _viewerDataProvider;

    public ViewerModel(IViewerPageDataProvider viewerDataProvider)
    {
        _viewerDataProvider = viewerDataProvider;
    }

    public string FaviconUrl { get; private set; } = string.Empty;
    public string LogoUrl { get; private set; } = string.Empty;
    public string PageDescription { get; private set; } = string.Empty;
    public string PageTitle { get; private set; } = string.Empty;
    public string ThemeUrl { get; private set; } = string.Empty;
    public string UserDisplayName { get; private set; } = string.Empty;
    public async Task OnGet()
    {
        var theme = await _viewerDataProvider.GetTheme(this);

        ThemeUrl = theme switch
        {
            ViewerPageTheme.Dark => "/_content/Remotely.Server/css/remote-control-dark.css",
            ViewerPageTheme.Light => "/_content/Remotely.Server/css/remote-control-light.css",
            _ => "/_content/Remotely.Server/css/remote-control-dark.css"
        };
        UserDisplayName = await _viewerDataProvider.GetUserDisplayName(this);
        PageTitle = await _viewerDataProvider.GetPageTitle(this);
        PageDescription = await _viewerDataProvider.GetPageDescription(this);
        FaviconUrl = await _viewerDataProvider.GetFaviconUrl(this);
        LogoUrl = await _viewerDataProvider.GetLogoUrl(this);
    }
}
