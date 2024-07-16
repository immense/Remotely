using Microsoft.AspNetCore.Mvc.Filters;
using Immense.RemoteControl.Server.Extensions;

namespace Immense.RemoteControl.Server.Abstractions;

/// <summary>
/// This service is used to determine if the current user is authorized 
/// to view the remote control page.  It gets registered as a scoped service
/// within <see cref="RemoteControlServerBuilder"/>.
/// </summary>
public interface IViewerAuthorizer
{
    /// <summary>
    /// Where the browser should be redirected if IsAuthorized returns false.
    /// Example: "/Account/Login"
    /// </summary>
    string UnauthorizedRedirectUrl { get; }

    /// <summary>
    /// Whether the current user is authorized to view the remote control page.
    /// Note: This does not inherently give access to any devices or resources.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<bool> IsAuthorized(AuthorizationFilterContext context);
}
