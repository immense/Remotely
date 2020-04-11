using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Utilities;

namespace Remotely.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .CaptureStartupErrors(true)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();

                    if (bool.TryParse(hostingContext.Configuration["ApplicationOptions:EnableWindowsEventLog"], out var enableEventLog))
                    {
                        if (EnvironmentHelper.IsWindows && enableEventLog)
                        {
                            logging.AddEventLog();
                        }
                    }
                });
    }
}
