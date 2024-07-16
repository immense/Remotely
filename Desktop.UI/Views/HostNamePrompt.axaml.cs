using Avalonia.Controls;

namespace Remotely.Desktop.UI.Views;

public partial class HostNamePrompt : Window
{
    public HostNamePrompt()
    {
        InitializeComponent();
    }

    public HostNamePromptViewModel? ViewModel => DataContext as HostNamePromptViewModel;
}
