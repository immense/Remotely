using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Microsoft.Extensions.Logging;

namespace Immense.RemoteControl.Desktop.UI.ViewModels;

public interface IMainWindowViewModel : IBrandedViewModelBase
{
}

public class MainWindowViewModel : BrandedViewModelBase, IMainWindowViewModel
{
    public MainWindowViewModel(
        IBrandingProvider brandingProvider,
        IUiDispatcher dispatcher,
        ILogger<BrandedViewModelBase> logger)
        : base(brandingProvider, dispatcher, logger)
    {
    }
}
