using Remotely.Desktop.Shared.Services;
using Remotely.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bitbound.SimpleMessenger;

namespace Remotely.Desktop.Shared.Startup;

public static class IServiceCollectionExtensions
{
    internal static void AddRemoteControlXplat(
        this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole().AddDebug();
        });

        services.AddSingleton<ISystemTime, SystemTime>();
        services.AddSingleton<IDesktopHubConnection, DesktopHubConnection>();
        services.AddSingleton<IIdleTimer, IdleTimer>();
        services.AddSingleton<IImageHelper, ImageHelper>();
        services.AddSingleton<IChatHostService, ChatHostService>();
        services.AddSingleton(s => WeakReferenceMessenger.Default);
        services.AddSingleton<IDesktopEnvironment, DesktopEnvironment>();
        services.AddSingleton<IDtoMessageHandler, DtoMessageHandler>();
        services.AddSingleton<IBrandingProvider, BrandingProvider>();
        services.AddSingleton<IAppState, AppState>();
        services.AddSingleton<IViewerFactory, ViewerFactory>();
        services.AddTransient<IScreenCaster, ScreenCaster>();
        services.AddTransient<IHubConnectionBuilder>(s => new HubConnectionBuilder());
    }
}
