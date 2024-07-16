using Immense.RemoteControl.Server.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Immense.RemoteControl.Server.Filters;

internal class ViewerAuthorizationFilter :  IAsyncAuthorizationFilter
{
    private readonly IViewerAuthorizer _authorizer;

    public ViewerAuthorizationFilter(IViewerAuthorizer authorizer)
    {
        _authorizer = authorizer;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (await _authorizer.IsAuthorized(context))
        {
            return;
        }

        context.Result = new RedirectResult(_authorizer.UnauthorizedRedirectUrl);
    }
}
