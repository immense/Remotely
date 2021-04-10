using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Remotely.Server.Models;
using Remotely.Server.Services;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Auth
{
    public class ExpiringTokenFilter : ActionFilterAttribute, IAuthorizationFilter
    {
        private readonly IExpiringTokenService _expiringTokenService;
        private readonly ILogger<ExpiringTokenFilter> _logger;

        public ExpiringTokenFilter(IExpiringTokenService expiringTokenService,
            ILogger<ExpiringTokenFilter> logger)
        {
            _expiringTokenService = expiringTokenService;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorization) &&
                _expiringTokenService.TryGetExpiration(authorization.ToString(), out var expiration) &&
                expiration > DateTimeOffset.Now)
            {
                _logger.LogDebug("Expiring token authorized.  Token: {token}.  Expiration: {expiration}", authorization, expiration);
                return;
            }

            _logger.LogDebug("Expiring token not authorized.  Token: {token}.", authorization);
            context.Result = new UnauthorizedResult();
        }
    }
}
