using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace Remotely.Desktop.Linux.Views
{
    public class FileTransferWindow : Window
    {
        public FileTransferWindow()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            Opened += FileTransferWindow_Opened;
        }

        private void FileTransferWindow_Opened(object sender, EventArgs e)
        {
            Topmost = false;

            var left = Screens.Primary.WorkingArea.Right - Width;

            var top = Screens.Primary.WorkingArea.Bottom - Height;

            Position = new PixelPoint((int)left, (int)top);
        }
    }
}
