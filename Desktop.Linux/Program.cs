using Immense.RemoteControl.Desktop.Shared.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Services;
using Immense.RemoteControl.Desktop.Shared.Services;
using System.Diagnostics;
using Remotely.Shared.Utilities;
using Immense.RemoteControl.Desktop.Shared.Startup;
using System.Linq;
using Immense.RemoteControl.Desktop.Linux.Startup;
using Immense.RemoteControl.Desktop.UI.Services;
using Avalonia;
using Immense.RemoteControl.Desktop.UI;
using Desktop.Shared.Services;

namespace Remotely.Desktop.XPlat;

public class Program
{
    // This is needed for the visual designer to work.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    public static async Task Main(string[] args)
    {
        var version = AppVersionHelper.GetAppVersion();
        var logger = new FileLogger("Remotely_Desktop", version, "Program.cs");
        var filePath = Environment.ProcessPath ?? Environment.GetCommandLineArgs().First();
        var serverUrl = Debugger.IsAttached ? "http://localhost:5000" : string.Empty;
        var getEmbeddedResult = await EmbeddedServerDataSearcher.Instance.TryGetEmbeddedData(filePath);
        if (getEmbeddedResult.IsSuccess)
        {
            serverUrl = getEmbeddedResult.Value.ServerUrl.AbsoluteUri;
        }
        else
        {
            logger.LogWarning(getEmbeddedResult.Exception, "Failed to extract embedded server data.");
        }

        var services = new ServiceCollection();

        services.AddSingleton<IOrganizationIdProvider, OrganizationIdProvider>();
        services.AddSingleton<IEmbeddedServerDataSearcher, EmbeddedServerDataSearcher>();

        services.AddRemoteControlLinux(
            config =>
            {
                config.AddBrandingProvider<BrandingProvider>();
            });

        services.AddLogging(builder =>
        {
            if (EnvironmentHelper.IsDebug)
            {
                builder.SetMinimumLevel(LogLevel.Debug);
            }
            builder.AddProvider(new FileLoggerProvider("Remotely_Desktop", version));
        });

        var provider = services.BuildServiceProvider();

        var appState = provider.GetRequiredService<IAppState>();
        var orgIdProvider = provider.GetRequiredService<IOrganizationIdProvider>();

        if (getEmbeddedResult.IsSuccess)
        {
            orgIdProvider.OrganizationId = getEmbeddedResult.Value.OrganizationId;
            appState.Host = getEmbeddedResult.Value.ServerUrl.AbsoluteUri;
        }

        if (appState.ArgDict.TryGetValue("org-id", out var orgId))
        {
            orgIdProvider.OrganizationId = orgId;
        }

        var result = await provider.UseRemoteControlClient(
            args,
            "The remote control client for Remotely.",
            serverUrl,
            false);

        if (!result.IsSuccess)
        {
            logger.LogError(result.Exception, "Failed to start remote control client.");
            Environment.Exit(1);
        }


        Console.WriteLine("Press Ctrl + C to exit.");

        var shutdownService = provider.GetRequiredService<IShutdownService>();
        Console.CancelKeyPress += async (s, e) =>
        {
            await shutdownService.Shutdown();
        };

        var dispatcher = provider.GetRequiredService<IUiDispatcher>();
        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, dispatcher.ApplicationExitingToken);
        }
        catch (TaskCanceledException) { }
    }
}