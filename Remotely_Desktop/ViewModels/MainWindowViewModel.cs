using Remotely_Desktop.Models;
using Remotely_Desktop.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_Desktop.ViewModels
{
    public class MainWindowViewModel
    {
        public ObservableCollection<Viewer> Viewers { get; set; }
        public string SessionID { get; set; } = "Retrieving...";

        private Process LaunchScreenCaster()
        {
            var filePath = ExtractScreenCasterEXE();
            return Process.Start(filePath, $"-mode Normal -host {Config.GetConfig().Host}");
        }
        private string ExtractScreenCasterEXE()
        {
            // Cleanup old files.
            foreach (var file in Directory.GetFiles(Path.GetTempPath(), "Remotely_ScreenCast*"))
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }

            // Get temp file name.
            var count = 0;
            var filePath = Path.Combine(Path.GetTempPath(), "Remotely_ScreenCast.exe");
            while (File.Exists(filePath))
            {
                filePath = Path.Combine(Path.GetTempPath(), $"Remotely_ScreenCast{count}.exe");
                count++;
            }

            // Extract ScreenCast.
            using (var mrs = Assembly.GetExecutingAssembly().GetManifestResourceStream("Remotely_Desktop.Resources.Remotely_ScreenCast.exe"))
            {
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    mrs.CopyTo(fs);
                }
            }
            return filePath;
        }
    }
}
