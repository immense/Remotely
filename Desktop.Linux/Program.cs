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
            builder.AddProvider(new FileLoggerProvider());
        });

        services.AddSingleton<IOrganizationIdProvider, OrganizationIdProvider>();
    },
    async services =>
    {
        var appState = services.GetRequiredService<IAppState>();
        if (appState.ArgDict.TryGetValue("org-id", out var orgId))
        {
            var orgIdProvider = services.GetRequiredService<IOrganizationIdProvider>();
            orgIdProvider.OrganizationId = orgId;
        }

        var brandingProvider = services.GetRequiredService<IBrandingProvider>();
        if (brandingProvider is BrandingProvider branding)
        {
            await branding.TrySetFromApi();
        }

    },
    AppConstants.ServerUrl);


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