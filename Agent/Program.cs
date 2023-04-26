using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Agent.Interfaces;
using Remotely.Agent.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Utilities;
using Remotely.Shared.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using Remotely.Agent.Services.Linux;
using Remotely.Agent.Services.MacOS;
using Remotely.Agent.Services.Windows;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace Remotely.Agent;

public class Program
{
    [Obsolete("Remove this when all services are in DI behind interfaces.")]
    public static IServiceProvider Services { get; set; }

    public static async Task Main(string[] args)
    {
        try
        {
            var host = Host
                .CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSystemd()
                .ConfigureServices(RegisterServices)
                .Build();

            await host.StartAsync();

            await Init(host.Services);

            await host.WaitForShutdownAsync();
        }
        catch (Exception ex)
        {
            var version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.0.0";
            var logger = new FileLogger("Remotely_Agent", version, "Main");
            logger.LogError(ex, "Error during agent startup.");
            throw;
        }
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddLogging(builder =>
        {
            builder.AddConsole().AddDebug();
            var version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.0.0";
            builder.AddProvider(new FileLoggerProvider("Remotely_Agent", version));
        });

        // TODO: All these should be registered as interfaces.
        services.AddSingleton<IAgentHubConnection, AgentHubConnection>();
        services.AddSingleton<ICpuUtilizationSampler, CpuUtilizationSampler>();
        services.AddHostedService(services => services.GetRequiredService<ICpuUtilizationSampler>());
        services.AddScoped<ChatClientService>();
        services.AddTransient<PSCore>();
        services.AddTransient<ExternalScriptingShell>();
        services.AddScoped<ConfigService>();
        services.AddScoped<Uninstaller>();
        services.AddScoped<ScriptExecutor>();
        services.AddScoped<IProcessInvoker, ProcessInvoker>();
        services.AddScoped<IUpdateDownloader, UpdateDownloader>();

        if (OperatingSystem.IsWindows())
        {
            services.AddScoped<IAppLauncher, AppLauncherWin>();
            services.AddSingleton<IUpdater, UpdaterWin>();
            services.AddSingleton<IDeviceInformationService, DeviceInfoGeneratorWin>();
        }
        else if (OperatingSystem.IsLinux())
        {
            services.AddScoped<IAppLauncher, AppLauncherLinux>();
            services.AddSingleton<IUpdater, UpdaterLinux>();
            services.AddSingleton<IDeviceInformationService, DeviceInfoGeneratorLinux>();
        }
        else if (OperatingSystem.IsMacOS())
        {
            services.AddScoped<IAppLauncher, AppLauncherMac>();
            services.AddSingleton<IUpdater, UpdaterMac>();
            services.AddSingleton<IDeviceInformationService, DeviceInfoGeneratorMac>();
        }
        else
        {
            throw new NotSupportedException("Operating system not supported.");
        }
    }

    private static async Task Init(IServiceProvider services)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, ex) =>
        {
            var logger = services.GetRequiredService<ILogger<AppDomain>>();
            if (ex.ExceptionObject is Exception exception)
            {
                logger.LogError(exception, "Unhandled exception in AppDomain.");
            }
            else
            {
                logger.LogError("Unhandled exception in AppDomain.");
            }
        };

        SetWorkingDirectory();

        await services.GetRequiredService<IUpdater>().BeginChecking();

        await services.GetRequiredService<IAgentHubConnection>().Connect();
    }

    private static void SetWorkingDirectory()
    {
        var exePath = Environment.ProcessPath ?? Environment.GetCommandLineArgs().First();
        var exeDir = Path.GetDirectoryName(exePath);
        Directory.SetCurrentDirectory(exeDir);
    }
}
