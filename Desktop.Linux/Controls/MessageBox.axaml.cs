using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Remotely.Desktop.Linux.ViewModels;
using Remotely.Shared.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Desktop.Linux.Controls
{
    public class MessageBox : Window
    {
        public static async Task<MessageBoxResult> Show(string message, string caption, MessageBoxType type)
        {
            var messageBox = new MessageBox();
            var viewModel = messageBox.DataContext as MessageBoxViewModel;
            viewModel.Caption = caption;
            viewModel.Message = message;

            switch (type)
            {
                case MessageBoxType.OK:
                    viewModel.IsOkButtonVisible = true;
                    break;
                case MessageBoxType.YesNo:
                    viewModel.AreYesNoButtonsVisible = true;
                    break;
                default:
                    break;
            }

            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                desktop.Windows.Count > 0)
            {
                await messageBox.ShowDialog(desktop.Windows.First());
            }
            else
            {
                var isClosed = false;
                messageBox.Closed += (sender, args) =>
                {
                    isClosed = true;
                };
                messageBox.Show();
                await TaskHelper.DelayUntilAsync(() => isClosed, TimeSpan.MaxValue);
            }
            return viewModel.Result;

        }
        public MessageBox()
        {
            // This doesn't appear to work when set in XAML.
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }



        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public enum MessageBoxType
    {
        OK,
        YesNo
    }

    public enum MessageBoxResult
    {
        Cancel,
        OK,
        Yes,
        No
    }
}
