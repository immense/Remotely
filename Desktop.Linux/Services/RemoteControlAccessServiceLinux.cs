using Avalonia.Threading;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Linux.ViewModels;
using Remotely.Desktop.Linux.Views;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Linux.Services
{
    public class RemoteControlAccessServiceLinux : IRemoteControlAccessService
    {
        public async Task<bool> PromptForAccess(string requesterName, string organizationName)
        {
            return await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var promptWindow = new PromptForAccessWindow();
                var viewModel = promptWindow.DataContext as PromptForAccessWindowViewModel;
                if (!string.IsNullOrWhiteSpace(requesterName))
                {
                    viewModel.RequesterName = requesterName;
                }
                if (!string.IsNullOrWhiteSpace(organizationName))
                {
                    viewModel.OrganizationName = organizationName;
                }

                var isOpen = true;
                promptWindow.Closed += (sender, arg) =>
                {
                    isOpen = false;
                };
                promptWindow.Show();
                while (isOpen)
                {
                    await Task.Delay(100);
                }
   
                return viewModel.PromptResult;
            });
        }
    }
}
