using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Remotely.Server.Services;

namespace Remotely.Server.Auth
{
    public class ApiAuthorizationFilter : ActionFilterAttribute, IAuthorizationFilter
    {
        public ApiAuthorizationFilter(IDataService dataService)
        {
            DataService = dataService;
        }

        private IDataService DataService { get; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var orgID = DataService.GetUserByNameWithOrg(context.HttpContext.User.Identity.Name)?.OrganizationID;
                context.HttpContext.Request.Headers["OrganizationID"] = orgID;
                return;
            }

            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var result))
            {
                var keyId = result.ToString().Split(":")[0]?.Trim();
                var apiSecret = result.ToString().Split(":")[1]?.Trim();

                if (DataService.ValidateApiKey(keyId, apiSecret, context.HttpContext.Request.Path, context.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    var orgID = DataService.GetApiKey(keyId)?.OrganizationID;
                    context.HttpContext.Request.Headers["OrganizationID"] = orgID;
                }
            }

            context.Result = new UnauthorizedResult();
        }
    }
}
