using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Remotely.Desktop.Unix.Services;

namespace Remotely.Desktop.Unix.ViewModels
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
