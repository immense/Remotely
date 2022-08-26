using Immense.RemoteControl.Server.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Remotely.Server.Services.RcImplementations
{
    public class ViewerAuthorizer : IViewerAuthorizer
    {
        public string UnauthorizedRedirectPageName => throw new System.NotImplementedException();

        public string UnauthorizedRedirectArea => throw new System.NotImplementedException();

        public bool IsAuthorized(AuthorizationFilterContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
