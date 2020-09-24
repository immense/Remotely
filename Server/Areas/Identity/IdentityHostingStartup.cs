using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Remotely.Server.Areas.Identity.IdentityHostingStartup))]
namespace Remotely.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}