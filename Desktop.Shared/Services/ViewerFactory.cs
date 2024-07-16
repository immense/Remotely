using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Immense.RemoteControl.Desktop.Shared.Services;

internal interface IViewerFactory
{
    IViewer CreateViewer(string viewerName, string viewerConnectionId);
}

internal class ViewerFactory : IViewerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ViewerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IViewer CreateViewer(string viewerName, string viewerConnectionId)
    {
        var desktopHubConnection = _serviceProvider.GetRequiredService<IDesktopHubConnection>();
        var screenCapturer = _serviceProvider.GetRequiredService<IScreenCapturer>();
        var clipboardService = _serviceProvider.GetRequiredService<IClipboardService>();
        var audioCapturer = _serviceProvider.GetRequiredService<IAudioCapturer>();
        var systemTime = _serviceProvider.GetRequiredService<ISystemTime>();
        var logger = _serviceProvider.GetRequiredService<ILogger<Viewer>>();

        return new Viewer(
            viewerName,
            viewerConnectionId,
            desktopHubConnection,
            screenCapturer,
            clipboardService,
            audioCapturer,
            systemTime,
            logger);
    }
}
