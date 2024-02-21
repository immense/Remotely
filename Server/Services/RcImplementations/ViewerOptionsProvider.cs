using Immense.RemoteControl.Server.Abstractions;
using Immense.RemoteControl.Server.Models;
using Immense.RemoteControl.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Remotely.Server.Services.RcImplementations;

public class ViewerOptionsProvider : IViewerOptionsProvider
{
    private readonly IDataService _dataService;

    public ViewerOptionsProvider(IDataService dataService) 
    {
        _dataService = dataService;
    }
    public async Task<RemoteControlViewerOptions> GetViewerOptions()
    {
        var settings = await _dataService.GetSettings();
        var options = new RemoteControlViewerOptions()
        {
            ShouldRecordSession = settings.EnableRemoteControlRecording
        };
        return options;
    }
}
