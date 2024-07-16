using Immense.RemoteControl.Desktop.Shared.Reactive;
using System.Windows.Input;

namespace Immense.RemoteControl.Desktop.UI.ViewModels.Fakes;

public class FakePromptForAccessViewModel : FakeBrandedViewModelBase, IPromptForAccessWindowViewModel
{
    public string OrganizationName { get; set; } = "Test Organization";
    public bool PromptResult { get; set; }
    public string RequesterName { get; set; } = "Test Requester";



    public ICommand CloseCommand => new RelayCommand(() => { });

    public ICommand MinimizeCommand => new RelayCommand(() => { });

    public string RequestMessage => "Test request message";

    public ICommand SetResultNo => new RelayCommand(() => { });

    public ICommand SetResultYes => new RelayCommand(() => { });
}
