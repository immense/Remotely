using Remotely.Agent.Installer.Win.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Remotely.Agent.Installer.Win
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void App_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Contains("-uninstall"))
            {
                try
                {
                    Logger.Write("Uninstall command received.  Preparing temp directory.");
                    var targetPath = Path.Combine(Path.GetTempPath(), "Remotely_Installer.exe");
                    File.Copy(Assembly.GetExecutingAssembly().Location, targetPath, true);
                    Logger.Write("Launching uninstaller.");
                    Process.Start(targetPath, "-rununinstall");
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }
        }
    }
}
