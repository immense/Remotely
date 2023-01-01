using Immense.RemoteControl.Desktop.Shared.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using System;
using Immense.RemoteControl.Desktop.Windows;
using Remotely.Desktop.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Services;
using Immense.RemoteControl.Desktop.Shared.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Immense.RemoteControl.Desktop.Shared.Enums;
using Immense.RemoteControl.Desktop.UI.Services;
using Remotely.Shared;
using System.Diagnostics;

var logger = new FileLogger("Remotely_Deskt", "Program.cs");
var filePath = Process.GetCurrentProcess()?.MainModule?.FileName;
var serverUrl = Debugger.IsAttached ? "https://localhost:5001" : string.Empty;
var getEmbeddedResult = await EmbeddedServerDataSearcher.Instance.TryGetEmbeddedData(filePath);
if (getEmbeddedResult.IsSuccess)
{
    serverUrl = getEmbeddedResult.Value.ServerUrl.AbsoluteUri;
}
else
{
    logger.LogWarning(getEmbeddedResult.Exception, "Failed to extract embedded server data.");
}

var provider = await Startup.UseRemoteControlClient(
    args,
    config =>
    {
        config.AddBrandingProvider<BrandingProvider>();
    },
    services =>
    {
        services.AddLogging(builder =>
        {
#if DEBUG
            builder.SetMinimumLevel(LogLevel.Debug);
#endif
            builder.AddProvider(new FileLoggerProvider("Remotely_Deskt"));
        });

        services.AddSingleton<IOrganizationIdProvider, OrganizationIdProvider>();
        services.AddSingleton<IEmbeddedServerDataSearcher, EmbeddedServerDataSearcher>();
    },
    services =>
    {
        var appState = services.GetRequiredService<IAppState>();
        if (appState.ArgDict.TryGetValue("org-id", out var orgId))
        {
            var orgIdProvider = services.GetRequiredService<IOrganizationIdProvider>();
            orgIdProvider.OrganizationId = orgId;
        }
        return Task.CompletedTask;
    },
    serverUrl);


Console.WriteLine("Press Ctrl + C to exit.");

var shutdownService = provider.GetRequiredService<IShutdownService>();
Console.CancelKeyPress += async (s, e) =>
{
    await shutdownService.Shutdown();
};

var dispatcher = provider.GetRequiredService<IAvaloniaDispatcher>();
try
{
    await Task.Delay(Timeout.InfiniteTimeSpan, dispatcher.AppCancellationToken);
}
catch (TaskCanceledException) { }