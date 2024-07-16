using Immense.RemoteControl.Server.Filters;
using Immense.RemoteControl.Server.Services;
using Immense.RemoteControl.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Immense.RemoteControl.Server.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds remote control services to an ASP.NET Core web app.  Remember to call
    /// <see cref="IApplicationBuilderExtensions.UseRemoteControlServer(Microsoft.AspNetCore.Builder.WebApplication)"/>
    /// after the WebApplication has been built.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure">Provides methods for adding required service implementations.</param>
    /// <returns></returns>
    public static IServiceCollection AddRemoteControlServer(
        this IServiceCollection services,
        Action<IRemoteControlServerBuilder> configure)
    {
        var builder = new RemoteControlServerBuilder(services);
        configure(builder);
        builder.Validate();

        services
            .AddSignalR()
            .AddMessagePackProtocol();

        services.AddHostedService<RemoteControlSessionCleaner>();
        services.AddHostedService<RemoteControlSessionReconnector>();
        services.AddSingleton<IDesktopStreamCache, DesktopStreamCache>();
        services.AddSingleton<IRemoteControlSessionCache, RemoteControlSessionCache>();
        services.AddSingleton<ISystemTime, SystemTime>();
        services.AddScoped<ViewerAuthorizationFilter>();

        return services;
    }
}
