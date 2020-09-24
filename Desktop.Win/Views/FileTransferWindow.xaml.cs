using System;
using System.Windows;
using System.Windows.Forms;

namespace Remotely.Desktop.Win.Views
{
    /// <summary>
    /// Interaction logic for FileTransferWindow.xaml
    /// </summary>
    public partial class FileTransferWindow : Window
    {

        public FileTransferWindow()
        {
            InitializeComponent();
            Left = Screen.PrimaryScreen.WorkingArea.Right - Width;
            Top = Screen.PrimaryScreen.WorkingArea.Bottom - Height;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Topmost = false;
        }
    }
}
