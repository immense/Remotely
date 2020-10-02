using Avalonia.Controls;
using ReactiveUI;
using Remotely.Desktop.Linux.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Remotely.Desktop.Linux.ViewModels
{
    public class PromptForAccessWindowViewModel : ReactiveViewModel
    {
        private string _organizationName = "your IT provider";
        private string _requesterName = "a technician";
        public ICommand CloseCommand => new Executor((param) =>
        {
            (param as Window)?.Close();
        });

        public ICommand MinimizeCommand => new Executor((param) =>
        {
            (param as Window).WindowState = WindowState.Minimized;
        });

        public string OrganizationName
        {
            get => _organizationName;
            set => this.RaiseAndSetIfChanged(ref _organizationName, value);
        }

        public bool PromptResult { get; set; }

        public string RequesterName
        {
            get => _requesterName;
            set => this.RaiseAndSetIfChanged(ref _requesterName, value);
        }
        public ICommand SetResultNo => new Executor(param =>
        {
            if (param is Window promptWindow)
            {
                PromptResult = false;
                promptWindow.Close();
            }
        });

        public ICommand SetResultYes => new Executor(param =>
        {
            if (param is Window promptWindow)
            {
                PromptResult = true;
                promptWindow.Close();
            }
        });
    }
}
