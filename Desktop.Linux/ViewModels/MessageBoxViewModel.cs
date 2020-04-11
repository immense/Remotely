using Avalonia.Controls;
using Remotely.Desktop.Linux.Controls;
using Remotely.Desktop.Linux.Services;
using System.Windows.Input;

namespace Remotely.Desktop.Linux.ViewModels
{
    public class MessageBoxViewModel : ViewModelBase
    {
        public string Caption { get; set; }
        public string Message { get; set; }

        public bool IsOkButtonVisible { get; set; }
        public bool AreYesNoButtonsVisible { get; set; }

        public MessageBoxResult Result { get; set; } = MessageBoxResult.Cancel;

        public ICommand OKCommand => new Executor((param) =>
        {
            Result = MessageBoxResult.OK;
            (param as Window).Close();
        });
        public ICommand YesCommand => new Executor((param) =>
        {
            Result = MessageBoxResult.Yes;
            (param as Window).Close();
        });
        public ICommand NoCommand => new Executor((param) =>
        {
            Result = MessageBoxResult.No;
            (param as Window).Close();
        });
    }
}
