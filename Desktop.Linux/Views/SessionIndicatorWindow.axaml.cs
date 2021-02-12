using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Linux.Controls;
using System;

namespace Remotely.Desktop.Linux.Views
{
    public class SessionIndicatorWindow : Window
    {
        public SessionIndicatorWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            Closing += SessionIndicatorWindow_Closing;
            PointerPressed += SessionIndicatorWindow_PointerPressed;
            Opened += SessionIndicatorWindow_Opened;
        }

        private void SessionIndicatorWindow_Opened(object sender, EventArgs e)
        {
            Topmost = false;

            var left = Screens.Primary.WorkingArea.Right - Width;

            var top = Screens.Primary.WorkingArea.Bottom - Height;

            Position = new PixelPoint((int)left, (int)top);
        }

        private void SessionIndicatorWindow_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == Avalonia.Input.PointerUpdateKind.LeftButtonPressed)
            {
                BeginMoveDrag(e);
            }
        }

        private async void SessionIndicatorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            var result = await MessageBox.Show("Stop the remote control session?", "Stop Session", MessageBoxType.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var shutdownService = ServiceContainer.Instance.GetRequiredService<IShutdownService>();
                await shutdownService.Shutdown();
            }
        }
    }
}
