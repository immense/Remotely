using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.UI.WPF.Services;
using System.Threading.Tasks;
using System.Threading;
using System;
using Immense.RemoteControl.Desktop.Windows;
using Remotely.Desktop.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Services;
using Immense.RemoteControl.Desktop.Shared.Services;
using System.Diagnostics;

var logger = new FileLogger("Remotely_Desktop", "Program.cs");
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
            builder.AddProvider(new FileLoggerProvider("Remotely_Desktop"));
        });

        services.AddSingleton<IOrganizationIdProvider, OrganizationIdProvider>();
        services.AddSingleton<IEmbeddedServerDataSearcher>(EmbeddedServerDataSearcher.Instance);
    },
    services =>
    {
        var appState = services.GetRequiredService<IAppState>();
        var orgIdProvider = services.GetRequiredService<IOrganizationIdProvider>();

        if (getEmbeddedResult.IsSuccess)
        {
            orgIdProvider.OrganizationId = getEmbeddedResult.Value.OrganizationId;
            appState.Host = getEmbeddedResult.Value.ServerUrl.AbsoluteUri;
        }

        if (appState.ArgDict.TryGetValue("org-id", out var orgId))
        {
            orgIdProvider.OrganizationId = orgId;
        }

        return Task.CompletedTask;
    },
    serverUrl);


var dispatcher = provider.GetRequiredService<IWindowsUiDispatcher>();

try
{
    await Task.Delay(Timeout.InfiniteTimeSpan, dispatcher.ApplicationExitingToken);
}
catch (TaskCanceledException) { }