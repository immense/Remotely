using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Remotely.Server.Services
{
    public class ClickOnceMiddleware
    {
        public static SemaphoreSlim AppFileLock { get; } = new(1,1);

        private readonly RequestDelegate _next;

        public ClickOnceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        
        public async Task Invoke(HttpContext context, IWebHostEnvironment env, IDataService dataService, ILogger<ClickOnceMiddleware> logger)
        {

            try
            {
                string architecture;
                string appFilePath;

                switch (context.Request.Path.Value)
                {
                    case "/Content/Win-x64/ClickOnce/Remotely_Desktop.application":
                        architecture = "x64";
                        appFilePath = Path.Combine(env.WebRootPath, "Content", "Win-x64", "ClickOnce", "Remotely_Desktop.application");
                        break;
                    case "/Content/Win-x86/ClickOnce/Remotely_Desktop.application":
                        architecture = "x86";
                        appFilePath = Path.Combine(env.WebRootPath, "Content", "Win-x86", "ClickOnce", "Remotely_Desktop.application");
                        break;
                    default:
                        await _next(context);
                        return;
                }

                var orgName = AppConstants.DefaultPublisherName;
                var productName = AppConstants.DefaultProductName;
                var orgId = string.Empty;

                var defaultOrg = await dataService.GetDefaultOrganization();
                if (defaultOrg != null)
                {
                    orgId = defaultOrg.ID;
                    if (!string.IsNullOrWhiteSpace(defaultOrg.OrganizationName))
                    {
                        orgName = defaultOrg.OrganizationName;
                    }

                    var brandingInfo = await dataService.GetBrandingInfo(defaultOrg.ID);
                    if (!string.IsNullOrWhiteSpace(brandingInfo?.Product))
                    {
                        productName = brandingInfo.Product;
                    }
                }

                var manifest = new XmlDocument();

                try
                {
                    await AppFileLock.WaitAsync();
                    manifest.Load(appFilePath);
                }
                finally
                {
                    AppFileLock.Release();
                }

                var deploymentProvider = manifest.GetElementsByTagName("deploymentProvider")[0];
                var codebaseValue = $"{context.Request.Scheme}://{context.Request.Host}/Content/Win-{architecture}/ClickOnce/Remotely_Desktop.application";
                if (!string.IsNullOrWhiteSpace(orgId))
                {
                    codebaseValue += $"?organizationid={orgId}";
                }
                deploymentProvider.Attributes["codebase"].Value = codebaseValue;

                var description = manifest.GetElementsByTagName("description")[0];
                description.Attributes["asmv2:publisher"].Value = orgName;
                description.Attributes["co.v1:suiteName"].Value = productName;
                description.Attributes["asmv2:product"].Value = productName + " Desktop";
                //manifest.Save(context.Response.Body);
                var contentBytes = Encoding.UTF8.GetBytes(manifest.OuterXml);
                await context.Response.BodyWriter.WriteAsync(contentBytes);

                await context.Response.CompleteAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ClickOnce middleware.");
                await _next(context);
            }
        }
    }
}
