using Avalonia.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Remotely.Desktop.Shared.Reactive;
using Remotely.Desktop.Shared.Services;

namespace Remotely.Desktop.UI.ViewModels;

public interface IHostNamePromptViewModel : IBrandedViewModelBase
{
    string Host { get; set; }
    ICommand OKCommand { get; }
}

public class HostNamePromptViewModel : BrandedViewModelBase, IHostNamePromptViewModel
{
    public HostNamePromptViewModel(
        IBrandingProvider brandingProvider,
        IUiDispatcher dispatcher,
        ILogger<HostNamePromptViewModel> logger)
        : base(brandingProvider, dispatcher, logger)
    {
        OKCommand = new RelayCommand<Window>(x => x?.Close());
    }

    public string Host
    {
        get => Get<string>() ?? "https://";
        set => Set(value);
    }

    public ICommand OKCommand { get; }
}
