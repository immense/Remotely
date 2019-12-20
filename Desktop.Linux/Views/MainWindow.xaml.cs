using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Remotely.Desktop.Linux.ViewModels;
using System.Threading.Tasks;

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

            _ = (this.DataContext as MainWindowViewModel).Init();
        }
    }
}
