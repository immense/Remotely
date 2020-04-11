using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Remotely.Desktop.Linux.ViewModels;

namespace Remotely.Desktop.Linux.Views
{
    public class MainWindow : Window
    {
        public static MainWindow Current { get; set; }
        public MainWindow()
        {
            Current = this;

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void TitleBanner_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == Avalonia.Input.PointerUpdateKind.LeftButtonPressed)
            {
                this.BeginMoveDrag(e);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<Border>("TitleBanner").PointerPressed += TitleBanner_PointerPressed;

            this.Opened += MainWindow_Opened;
        }

        private async void MainWindow_Opened(object sender, System.EventArgs e)
        {
            await (this.DataContext as MainWindowViewModel).Init();
        }
    }
}
