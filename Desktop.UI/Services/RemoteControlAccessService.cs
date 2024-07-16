using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Shared.Enums;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Immense.RemoteControl.Desktop.UI.Services;

public class RemoteControlAccessService : IRemoteControlAccessService
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IUiDispatcher _dispatcher;
    private readonly ILogger<RemoteControlAccessService> _logger;
    private volatile int _promptCount = 0;

    public RemoteControlAccessService(
        IViewModelFactory viewModelFactory,
        IUiDispatcher dispatcher,
        ILogger<RemoteControlAccessService> logger)
    {
        _viewModelFactory = viewModelFactory;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public bool IsPromptOpen => _promptCount > 0;

    public async Task<PromptForAccessResult> PromptForAccess(string requesterName, string organizationName)
    {
        return await _dispatcher.InvokeAsync(async () =>
        {
            try
            {
                Interlocked.Increment(ref _promptCount);
                var viewModel = _viewModelFactory.CreatePromptForAccessViewModel(requesterName, organizationName);
                var promptWindow = new PromptForAccessWindow()
                {
                    DataContext = viewModel
                };

                var result = await _dispatcher.Show(promptWindow, TimeSpan.FromMinutes(1));

                if (!result)
                {
                    return PromptForAccessResult.TimedOut;
                }
                
                return viewModel.PromptResult ?
                    PromptForAccessResult.Accepted :
                    PromptForAccessResult.Denied;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while prompting for remote control access.");
                return PromptForAccessResult.Error;
            }
            finally
            {
                Interlocked.Decrement(ref _promptCount);
            }
        });
    }
}
