using Immense.RemoteControl.Desktop.Shared.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using System;
using Remotely.Desktop.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Services;
using Immense.RemoteControl.Desktop.Shared.Services;
using Immense.RemoteControl.Desktop.UI.Services;
using System.Diagnostics;
using Immense.RemoteControl.Desktop.Linux.Startup;
using Remotely.Shared.Utilities;

var version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.0.0";
var logger = new FileLogger("Remotely_Desktop", version, "Program.cs");
var filePath = Process.GetCurrentProcess()?.MainModule?.FileName;
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