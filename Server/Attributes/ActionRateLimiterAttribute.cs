using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net;

namespace Remotely.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionRateLimiterAttribute : ActionFilterAttribute
    {
        public string Action { get; set; }
        public int TimeoutInSeconds { get; set; } = 5;
        private static MemoryCache RequestCache { get; } = new MemoryCache(new MemoryCacheOptions());


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ip = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
            var key = $"Action-{ip}";

            if (!RequestCache.TryGetValue(key, out _))
            {
                RequestCache.Set(key, true, TimeSpan.FromSeconds(TimeoutInSeconds));
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            }
            base.OnActionExecuting(context);
        }
    }
}
