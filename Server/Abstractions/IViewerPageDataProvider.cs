using Immense.RemoteControl.Server.Areas.RemoteControl.Pages;
using Immense.RemoteControl.Server.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Immense.RemoteControl.Server.Extensions;

namespace Immense.RemoteControl.Server.Abstractions;

/// <summary>
/// This service is used to provider UI data to the remote control page.
/// It gets registered as a scoped service within <see cref="RemoteControlServerBuilder"/>.
/// </summary>
public interface IViewerPageDataProvider
{
    Task<ViewerPageTheme> GetTheme(PageModel pageModel);
    Task<string> GetUserDisplayName(PageModel pageModel);
    Task<string> GetPageTitle(PageModel pageModel);
    Task<string> GetPageDescription(PageModel pageModel);
    Task<string> GetFaviconUrl(PageModel pageModel);
    Task<string> GetLogoUrl(PageModel pageModel);
}
