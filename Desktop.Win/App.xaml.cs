using Microsoft.Extensions.DependencyInjection;
using Remotely.Desktop.Win.Services;
using Remotely.Desktop.Win.ViewModels;
using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Services;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace Remotely.Desktop.Win
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Write(e.Exception);
            MessageBox.Show("There was an unhandled exception.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
            foreach (var viewer in conductor.Viewers.Values.ToArray())
            {
                try
                {
                    viewer.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (Environment.GetCommandLineArgs().Contains("-elevate"))
            {
                var commandLine = Win32Interop.GetCommandLine().Replace(" -elevate", "").Replace("\"", "");

                Logger.Write($"Elevating process {commandLine}.");
                var result = Win32Interop.OpenInteractiveProcess(
                    commandLine,
                    "default",
                    false,
                    out var procInfo);
                Logger.Write($"Elevate result: {result}. Process ID: {procInfo.dwProcessId}.");
                Environment.Exit(0);
            }
        }
    }
}
