using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Remotely.Agent.Installer.Win.Services
{
    public class InstallerService
    {
        public bool Install()
        {
            try
            {
                if (!CheckIsAdministrator())
                {
                    return false;
                }

                InstallService();

                var programPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Remotely", "ScreenCast", "Remotely_ScreenCast.exe");
                Process.Start("netsh", "advfirewall firewall delete rule name=\"Remotely ScreenCast\"").WaitForExit();
                Process.Start("netsh", $"advfirewall firewall add rule name=\"Remotely ScreenCast\" program=\"{programPath}\" protocol=any dir=in enable=yes action=allow profile=Private,Domain description=\"The agent that allows screen sharing and remote control for Remotely.\"").WaitForExit();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }

        }

        public async Task<bool> Uninstall()
        {
            try
            {
                if (!CheckIsAdministrator())
                {
                    return false;
                }

                Process.Start("cmd.exe", "/c sc delete Remotely_Service").WaitForExit();

                var procs = Process.GetProcessesByName("Remotely_Agent").Concat(Process.GetProcessesByName("Remotely_ScreenCast"));

                foreach (var proc in procs)
                {
                    proc.Kill();
                }

                await Task.Delay(500);

                Directory.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Remotely"), true);

                Process.Start("netsh", "advfirewall firewall delete rule name=\"Remotely ScreenCast\"").WaitForExit();

                return true;

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        private bool CheckIsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            var result = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!result)
            {
                MessageBox.Show("Elevated privileges are required.  Please restart the installer using 'Run as administrator'.", "Elevation Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result;
        }

        private void InstallService()
        {
            Logger.Write("Install started.");
            var installPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var serv = ServiceController.GetServices().FirstOrDefault(ser => ser.ServiceName == "Remotely_Service");
            if (serv == null)
            {
                var command = new string[] { "/assemblypath=" + installPath };
                var serviceInstaller = new ServiceInstaller();
                var context = new InstallContext("", command);
                serviceInstaller.Context = context;
                serviceInstaller.DisplayName = "Remotely Service";
                serviceInstaller.Description = "Background service that maintains a connection to the Remotely server.  The service is used for remote support and maintenance by this computer's administrators.";
                serviceInstaller.ServiceName = "Remotely_Service";
                serviceInstaller.StartType = ServiceStartMode.Automatic;
                serviceInstaller.Parent = new ServiceProcessInstaller();

                var state = new System.Collections.Specialized.ListDictionary();
                serviceInstaller.Install(state);
                Logger.Write("Service installed.");
            }
            serv = ServiceController.GetServices().FirstOrDefault(ser => ser.ServiceName == "Remotely_Service");
            if (serv.Status != ServiceControllerStatus.Running)
            {
                serv.Start();
            }
            Logger.Write("Service started.");
            var psi = new ProcessStartInfo("cmd.exe", "/c sc.exe failure \"Remotely_Service\" reset=5 actions=restart/5000");
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(psi).WaitForExit();
        }
    }
}
