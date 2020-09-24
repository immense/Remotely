using Avalonia.Controls;
using ReactiveUI;
using Remotely.Desktop.Linux.Services;
using System.Windows.Input;

namespace Remotely.Desktop.Linux.ViewModels
{
    public class HostNamePromptViewModel : ReactiveViewModel
    {
        public string _host;

        public string Host
        {
            get => _host;
            set => this.RaiseAndSetIfChanged(ref _host, value);
        }

        public ICommand OKCommand => new Executor((param) =>
        {
            (param as Window).Close();
        });
    }
}
