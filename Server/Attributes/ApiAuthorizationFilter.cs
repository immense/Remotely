using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Remotely.Server.Services;

namespace Remotely.Server.Attributes
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
                var orgID = DataService.GetUserByName(context.HttpContext.User.Identity.Name)?.OrganizationID;
                context.HttpContext.Request.Headers.Add("OrganizationID", orgID);
                return;
            }

            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var result))
            {
                var apiToken = result.ToString().Split(":")[0]?.Trim();
                var apiSecret = result.ToString().Split(":")[1]?.Trim();

                if (DataService.ValidateApiToken(apiToken, apiSecret, context.HttpContext.Request.Path, context.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    var orgID = DataService.GetApiToken(apiToken)?.OrganizationID;

                    // In case the filter gets run twice.
                    if (context.HttpContext.Request.Headers.ContainsKey("OrganizationID"))
                    {
                        return;
                    }
                    context.HttpContext.Request.Headers.Add("OrganizationID", orgID);
                    return;
                }
            }

            context.Result = new UnauthorizedResult();
        }
    }
}
