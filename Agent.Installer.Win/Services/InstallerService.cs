using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Agent.Installer.Win.Services
{
    public class InstallerService
    {
        private void InstallService(List<string> args)
        {
            try
            {
                Logger.Write("Install started.");
                var installPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var serv = ServiceController.GetServices().FirstOrDefault(ser => ser.ServiceName == "Remotely_Service");
                if (serv == null)
                {
                    string[] command = new String[] { "/assemblypath=" + installPath };
                    ServiceInstaller ServiceInstallerObj = new ServiceInstaller();
                    InstallContext Context = new InstallContext("", command);
                    ServiceInstallerObj.Context = Context;
                    ServiceInstallerObj.DisplayName = "Remotely Service";
                    ServiceInstallerObj.Description = "Background service that maintains a connection to the Remotely server.  The service is used for remote support and maintenance by this computer's administrators.";
                    ServiceInstallerObj.ServiceName = "Remotely_Service";
                    ServiceInstallerObj.StartType = ServiceStartMode.Automatic;
                    ServiceInstallerObj.DelayedAutoStart = true;
                    ServiceInstallerObj.Parent = new ServiceProcessInstaller();

                    System.Collections.Specialized.ListDictionary state = new System.Collections.Specialized.ListDictionary();
                    ServiceInstallerObj.Install(state);
                }
                serv = ServiceController.GetServices().FirstOrDefault(ser => ser.ServiceName == "Remotely_Service");
                if (serv != null && serv.Status != ServiceControllerStatus.Running)
                {
                    serv.Start();
                }
                var psi = new ProcessStartInfo("cmd.exe", "/c sc.exe failure \"Remotely_Service\" reset=5 actions=restart/5000");
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(psi).WaitForExit();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
