using Immense.RemoteControl.Server.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Immense.RemoteControl.Server.Extensions;

public interface IRemoteControlServerBuilder
{
    /// <summary>
    /// Adds your implementation of <see cref="IHubEventHandler"/> to the
    /// DI container as a singleton.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    void AddHubEventHandler<T>()
        where T : class, IHubEventHandler;

    /// <summary>
    /// Adds your implementation of <see cref="ISessionRecordingSink"/> to the
    /// DI container as a singleton.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    void AddSessionRecordingSink<T>()
        where T : class, ISessionRecordingSink;

    /// <summary>
    /// Adds your implementation of <see cref="IViewerAuthorizer"/> to the
    /// DI container as a scoped service.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    void AddViewerAuthorizer<T>()
        where T : class, IViewerAuthorizer;

    /// <summary>
    /// Adds your implementation of <see cref="IViewerOptionsProvider"/> to the
    /// DI container as a singleton.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    void AddViewerOptionsProvider<T>()
        where T : class, IViewerOptionsProvider;

    /// <summary>
    /// Adds your implementation of <see cref="IViewerPageDataProvider"/> to the
    /// DI container as a scoped service.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    void AddViewerPageDataProvider<T>()
        where T : class, IViewerPageDataProvider;
}

internal class RemoteControlServerBuilder : IRemoteControlServerBuilder
{
    private readonly IServiceCollection _services;

    public RemoteControlServerBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public void AddHubEventHandler<T>() 
        where T : class, IHubEventHandler
    {
        _services.AddSingleton<IHubEventHandler, T>();
    }

    public void AddSessionRecordingSink<T>()
        where T : class, ISessionRecordingSink
    {
        _services.AddSingleton<ISessionRecordingSink, T>();
    }

    public void AddViewerAuthorizer<T>()
            where T : class, IViewerAuthorizer
    {
        _services.AddScoped<IViewerAuthorizer, T>();
    }

    public void AddViewerOptionsProvider<T>()
        where T : class, IViewerOptionsProvider
    {
        _services.AddSingleton<IViewerOptionsProvider, T>();
    }

    public void AddViewerPageDataProvider<T>()
            where T : class, IViewerPageDataProvider
    {
        _services.AddScoped<IViewerPageDataProvider, T>();
    }

    internal void Validate()
    {
        var serviceTypes = new[]
        {
            typeof(IHubEventHandler),
            typeof(IViewerAuthorizer),
            typeof(IViewerPageDataProvider),
            typeof(ISessionRecordingSink),
            typeof(IViewerOptionsProvider)
        };

        foreach (var type in serviceTypes)
        {
            if (!_services.Any(x => x.ServiceType == type))
            {
                throw new Exception($"Missing service registration for type {type.Name}.");
            }
        }
    }
}
