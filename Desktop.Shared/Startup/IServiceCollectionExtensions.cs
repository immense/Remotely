using Immense.RemoteControl.Desktop.Shared.Services;
using Immense.RemoteControl.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bitbound.SimpleMessenger;
using Desktop.Shared.Services;

namespace Immense.RemoteControl.Desktop.Shared.Startup;

public static class IServiceCollectionExtensions
{
    internal static void AddRemoteControlXplat(
        this IServiceCollection services,
        Action<IRemoteControlClientBuilder> clientConfig)
    {
        var builder = new RemoteControlClientBuilder(services);
        clientConfig.Invoke(builder);
        builder.Validate();

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
        services.AddSingleton<IAppState, AppState>();
        services.AddSingleton<IViewerFactory, ViewerFactory>();
        services.AddTransient<IScreenCaster, ScreenCaster>();
        services.AddTransient<IHubConnectionBuilder>(s => new HubConnectionBuilder());
    }
}
