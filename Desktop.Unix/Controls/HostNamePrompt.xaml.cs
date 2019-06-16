using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Remotely.Desktop.Unix.Controls
{
    public class HostNamePrompt : Window
    {
        public HostNamePrompt()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.Icon = App.Current?.MainWindow?.Icon;
        }
    }
}
