using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Remotely.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Auth
{
    public class ApiAuthorizationFilter : ActionFilterAttribute, IAuthorizationFilter
    {
        public ApiAuthorizationFilter(DataService dataService)
        {
            DataService = dataService;
        }

        private DataService DataService { get; }

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
                
                if (DataService.ValidateApiToken(apiToken, apiSecret, context.HttpContext.Request.Path))
                {
                    var orgID = DataService.GetApiToken(apiToken)?.OrganizationID;
                    context.HttpContext.Request.Headers.Add("OrganizationID", orgID);
                    return;
                }
            }

            context.Result = new UnauthorizedResult();
        }
    }
}
