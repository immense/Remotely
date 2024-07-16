using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Immense.RemoteControl.Desktop.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Immense.RemoteControl.Desktop.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        if (!Design.IsDesignMode)
        {
            DataContext = StaticServiceProvider.Instance?.GetService<IMainWindowViewModel>();
        }

        InitializeComponent();
        Closed += MainWindow_Closed;
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        var dispatcher = StaticServiceProvider.Instance?.GetService<IUiDispatcher>();
        dispatcher?.Shutdown();
    }
}
