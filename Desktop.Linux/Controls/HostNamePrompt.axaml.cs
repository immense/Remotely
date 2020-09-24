using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Remotely.Desktop.Linux.ViewModels;

namespace Remotely.Desktop.Linux.Controls
{
    public class HostNamePrompt : Window
    {
        public HostNamePrompt()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public HostNamePromptViewModel ViewModel => DataContext as HostNamePromptViewModel;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
