using Avalonia.Controls;
using ReactiveUI;
using Remotely.Desktop.Linux.Controls;
using Remotely.Desktop.Linux.Services;
using System.Windows.Input;

namespace Remotely.Desktop.Linux.ViewModels
{
    public class MessageBoxViewModel : BrandedViewModelBase
    {
        private bool areYesNoButtonsVisible;
        private string caption;
        private bool isOkButtonVisible;
        private string message;
        public bool AreYesNoButtonsVisible
        {
            get => areYesNoButtonsVisible;
            set => this.RaiseAndSetIfChanged(ref areYesNoButtonsVisible, value);
        }

        public string Caption
        {
            get => caption;
            set => this.RaiseAndSetIfChanged(ref caption, value);
        }

        public bool IsOkButtonVisible
        {
            get => isOkButtonVisible;
            set => this.RaiseAndSetIfChanged(ref isOkButtonVisible, value);
        }

        public string Message
        {
            get => message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }
        public ICommand NoCommand => new Executor((param) =>
        {
            Result = MessageBoxResult.No;
            (param as Window).Close();
        });

        public ICommand OKCommand => new Executor((param) =>
        {
            Result = MessageBoxResult.OK;
            (param as Window).Close();
        });

        public MessageBoxResult Result { get; set; } = MessageBoxResult.Cancel;
        public ICommand YesCommand => new Executor((param) =>
        {
            Result = MessageBoxResult.Yes;
            (param as Window).Close();
        });
    }
}
