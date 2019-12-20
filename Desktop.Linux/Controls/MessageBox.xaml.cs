using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Remotely.Desktop.Linux.ViewModels;
using Remotely.Desktop.Linux.Views;
using System.Threading.Tasks;

namespace Remotely.Desktop.Linux.Controls
{
    public class MessageBox : Window
    {
        public static async Task<MessageBoxResult> Show(string message, string caption, MessageBoxType type)
        {
            var messageBox = new MessageBox();
            var viewModel = new MessageBoxViewModel();
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

            messageBox.DataContext = viewModel;

            await messageBox.ShowDialog(MainWindow.Current);

            return viewModel.Result;
        }
        public MessageBox()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }



        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.Icon = MainWindow.Current?.Icon;
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
