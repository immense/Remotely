using Microsoft.Extensions.DependencyInjection;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Remotely.Desktop.Win.Services
{
    public class ShutdownServiceWin : IShutdownService
    {
        public async Task Shutdown()
        {
            try
            {
                Logger.Write($"Exiting process ID {Environment.ProcessId}.");
                var casterSocket = ServiceContainer.Instance.GetRequiredService<ICasterSocket>();
                await casterSocket.DisconnectAllViewers();
                await casterSocket.Disconnect();
                System.Windows.Forms.Application.Exit();
                App.Current.Dispatcher.Invoke(() =>
                {
                    App.Current.Shutdown();
                });
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
