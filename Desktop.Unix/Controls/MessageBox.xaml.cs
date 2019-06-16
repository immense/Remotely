using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Remotely.Desktop.Unix.ViewModels;
using System.Threading.Tasks;

namespace Remotely.Desktop.Unix.Controls
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

            await messageBox.ShowDialog(App.Current.MainWindow);

            return viewModel.Result;
        }
        private MessageBox()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }



        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.Icon = App.Current?.MainWindow?.Icon;
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
