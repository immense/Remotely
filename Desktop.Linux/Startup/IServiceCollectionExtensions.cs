using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.Shared.Startup;
using Microsoft.Extensions.DependencyInjection;
using Immense.RemoteControl.Desktop.Linux.Services;
using Immense.RemoteControl.Desktop.UI.ViewModels;
using Immense.RemoteControl.Desktop.UI.Services;
using Immense.RemoteControl.Desktop.UI.Startup;

namespace Immense.RemoteControl.Desktop.Linux.Startup;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds Linux and cross-platform remote control services to the service collection.
    /// All methods on <see cref="IRemoteControlClientBuilder"/> must be called to register
    /// required services.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="clientConfig"></param>
    public static void AddRemoteControlLinux(
        this IServiceCollection services,
        Action<IRemoteControlClientBuilder> clientConfig)
    {
        services.AddRemoteControlXplat(clientConfig);
        services.AddRemoteControlUi();

        services.AddSingleton<IAppStartup, AppStartup>();
        services.AddSingleton<ICursorIconWatcher, CursorIconWatcherLinux>();
        services.AddSingleton<IKeyboardMouseInput, KeyboardMouseInputLinux>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        services.AddSingleton<IAudioCapturer, AudioCapturerLinux>();
        services.AddTransient<IScreenCapturer, ScreenCapturerLinux>();
        services.AddScoped<IFileTransferService, FileTransferServiceLinux>();
        services.AddSingleton<ISessionIndicator, SessionIndicator>();
        services.AddSingleton<IShutdownService, ShutdownServiceLinux>();
    }
}
