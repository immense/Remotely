using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Remotely.Desktop.Linux.Services;

namespace Remotely.Desktop.Linux.ViewModels
{
    public class HostNamePromptViewModel : ViewModelBase
    {
        public static HostNamePromptViewModel Current { get; private set; }
        public HostNamePromptViewModel()
        {
            Current = this;
        }

        public string host;
        public string Host
        {
            get => host;
            set => this.RaiseAndSetIfChanged(ref host, value);
        }

        public ICommand OKCommand => new Executor((param) =>
        {
            (param as Window).Close();
        });
    }
}
