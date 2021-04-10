using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.CaptureStartupErrors(true);
                    webBuilder.ConfigureLogging((hostingContext, logging) =>
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
                });
    }
}
