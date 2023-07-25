using Immense.RemoteControl.Shared.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Remotely.Server.Services;
using Remotely.Shared;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Server.Auth;

public class ApiAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly IDataService _dataService;
    private readonly ILogger<ApiAuthorizationFilter> _logger;

    public ApiAuthorizationFilter(
        IDataService dataService, 
        ILogger<ApiAuthorizationFilter> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            await Authorize(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while authorizing API key.");
        }
    }

    private async Task Authorize(AuthorizationFilterContext context)
    {
        var http = context.HttpContext;
        http.Request.Headers["OrganizationID"] = string.Empty;

        if (http.User.Identity?.IsAuthenticated == true)
        {
            var userResult = await _dataService.GetUserByName($"{http.User.Identity.Name}");
            if (userResult.IsSuccess && userResult.Value.IsAdministrator)
            {
                http.Request.Headers["OrganizationID"] = userResult.Value.OrganizationID;
                return;
            }
        }

        if (http.Request.Headers.TryGetValue(AppConstants.ApiKeyHeaderName, out var apiHeaderValue))
        {
            var headerComponents = apiHeaderValue.ToString().Split(":");
            if (headerComponents.Length < 2)
            {
                http.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Result = new UnauthorizedResult();
                return;
            };

            var keyId = headerComponents[0].Trim();
            var secret = headerComponents[1].Trim();

            var isValid = await _dataService.ValidateApiKey(
                       keyId,
                       secret,
                       http.Request.Path,
                       $"{http.Connection.RemoteIpAddress}");

            if (isValid)
            {
                var keyResult = await _dataService.GetApiKey(keyId);

                if (keyResult.IsSuccess)
                {
                    http.Request.Headers["OrganizationID"] = keyResult.Value.OrganizationID;
                    return;
                }
            }
        }

        http.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        context.Result = new UnauthorizedResult();
    }
}
