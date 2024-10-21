using Avalonia;
using Avalonia.Controls;

namespace Remotely.Desktop.UI.Views;

public partial class SessionIndicatorWindow : Window
{
    public SessionIndicatorWindow()
    {
        InitializeComponent();

        Closing += SessionIndicatorWindow_Closing;
        PointerPressed += SessionIndicatorWindow_PointerPressed;
        Opened += SessionIndicatorWindow_Opened;
    }

    private void SessionIndicatorWindow_Opened(object? sender, EventArgs e)
    {
        Topmost = false;

        if (Screens.Primary is not null)
        {
            var left = Screens.Primary.WorkingArea.Right - FrameSize?.Width ?? Width;
            var top = Screens.Primary.WorkingArea.Bottom - FrameSize?.Height ?? Height;
            Position = new PixelPoint((int)left, (int)top);
        }
    }

    private void SessionIndicatorWindow_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == Avalonia.Input.PointerUpdateKind.LeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }

    private async void SessionIndicatorWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        // This event appears to fire in design mode too.
        if (Design.IsDesignMode)
        {
            return;
        }

        if (DataContext is ISessionIndicatorWindowViewModel viewModel)
        {
            e.Cancel = true;
            await viewModel.PromptForExit();
        }
    }
}
