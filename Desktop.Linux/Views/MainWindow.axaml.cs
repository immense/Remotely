using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Remotely.Desktop.Linux.ViewModels;

namespace Remotely.Desktop.Linux.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            Current = this;
            
            InitializeComponent();
        }

        public static MainWindow Current { get; set; }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<Border>("TitleBanner").PointerPressed += TitleBanner_PointerPressed;

            Opened += MainWindow_Opened;
        }

        private async void MainWindow_Opened(object sender, System.EventArgs e)
        {
            await (DataContext as MainWindowViewModel).Init();
        }

        private void TitleBanner_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == Avalonia.Input.PointerUpdateKind.LeftButtonPressed)
            {
                BeginMoveDrag(e);
            }
        }
    }
}
