using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Remotely.Desktop.Unix.ViewModels;
using System.Threading.Tasks;

namespace Remotely.Desktop.Unix.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void TitleBanner_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (e.MouseButton == Avalonia.Input.MouseButton.Left)
            {
                this.BeginMoveDrag();
            }
        }

        private async void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<Border>("TitleBanner").PointerPressed += TitleBanner_PointerPressed;

            while (App.Current.MainWindow == null)
            {
                await Task.Delay(1);
            }
            await MainWindowViewModel.Current.Init();
        }
    }
}
