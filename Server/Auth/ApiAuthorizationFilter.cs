using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Remotely.Server.Services;
using System;
using System.Net;
using System.Text;

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

                var headerComponents = result.ToString().Split(" ");
                if (headerComponents.Length < 2)
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                };

                var tokenType = headerComponents[0].Trim();
                var encodedToken = headerComponents[1].Trim();

                switch (tokenType)
                {
                    case "Basic":
                        byte[] data = Convert.FromBase64String(encodedToken);
                        string decodedString = Encoding.UTF8.GetString(data);

                        var authComponents = decodedString.ToString().Split(":");
                        if (authComponents.Length < 2)
                        {
                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            return;
                        };

                        var keyId = authComponents[0]?.Trim();
                        var apiSecret = authComponents[1]?.Trim();
                        if (DataService.ValidateApiKey(keyId, apiSecret, context.HttpContext.Request.Path, context.HttpContext.Connection.RemoteIpAddress.ToString()))
                        {
                            var orgID = DataService.GetApiKey(keyId)?.OrganizationID;
                            context.HttpContext.Request.Headers["OrganizationID"] = orgID;
                            return;
                        }
                        break;
                }

            }

            context.Result = new UnauthorizedResult();
        }
    }
}
