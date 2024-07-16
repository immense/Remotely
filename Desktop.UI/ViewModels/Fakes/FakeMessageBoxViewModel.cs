using Immense.RemoteControl.Desktop.Shared.Reactive;
using System.Windows.Input;
using Immense.RemoteControl.Desktop.UI.Controls.Dialogs;

namespace Immense.RemoteControl.Desktop.UI.ViewModels.Fakes;

public class FakeMessageBoxViewModel : FakeBrandedViewModelBase, IMessageBoxViewModel
{
    public bool AreYesNoButtonsVisible { get; set; } = true;
    public string Caption { get; set; } = "Test Caption";
    public bool IsOkButtonVisible { get; set; } = false;
    public string Message { get; set; } = "This is a test message.";

    public ICommand NoCommand => new RelayCommand(() => { });

    public ICommand OKCommand => new RelayCommand(() => { });

    public MessageBoxResult Result { get; set; } = MessageBoxResult.Yes;

    public ICommand YesCommand => new RelayCommand(() => { });
}
