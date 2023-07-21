using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Server.Models;
using Immense.RemoteControl.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Remotely.Server.Services.RcImplementations;

public class ViewerOptionsProvider : IViewerOptionsProvider
{
    private readonly IApplicationConfig _appConfig;

    public ViewerOptionsProvider(IApplicationConfig appConfig) 
    {
        _appConfig = appConfig;
    }
    public Task<RemoteControlViewerOptions> GetViewerOptions()
    {
        var options = new RemoteControlViewerOptions()
        {
            ShouldRecordSession = _appConfig.EnableRemoteControlRecording
        };
        return Task.FromResult(options);
    }
}
