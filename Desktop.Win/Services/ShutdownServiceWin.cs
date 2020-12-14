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
            Logger.Debug($"Exiting process ID {Process.GetCurrentProcess().Id}.");
            var casterSocket = ServiceContainer.Instance.GetRequiredService<CasterSocket>();
            await casterSocket.DisconnectAllViewers();
            await casterSocket.Disconnect();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            });
            System.Windows.Forms.Application.Exit();
            Environment.Exit(0);
        }
    }
}
