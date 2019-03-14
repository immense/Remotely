using Remotely_Desktop.Controls;
using Remotely_Desktop.Models;
using Remotely_Desktop.Services;
using Remotely_ScreenCast.Models;
using Remotely_ScreenCast.Sockets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Viewer = Remotely_Desktop.Models.Viewer;

namespace Remotely_Desktop.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            Current = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static MainWindowViewModel Current { get; private set; }
        public Config Config { get; private set; }
        public string Host
        {
            get
            {
                return Config?.Host;
            }
        }

        public IPC IPC { get; set; } = new IPC();
        public Process ScreenCasterProcess { get; set; }
        public string SessionID { get; set; } = "Retrieving...";
        public ObservableCollection<Viewer> Viewers { get; set; }
        public void FirePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Init()
        {
            var filePath = ExtractScreenCasterEXE();
            Config = Config.GetConfig();
            while (string.IsNullOrWhiteSpace(Config.Host))
            {
                PromptForHostName();
            }
            var psi = new ProcessStartInfo()
            {
                FileName = filePath,
                Arguments = $"-mode Normal -host {Config.Host}",
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            MessageHandlers.SessionIDChanged += SessionIDChanged;
            MessageHandlers.ViewerRemoved += ViewerRemoved;
            MessageHandlers.ViewerAdded += ViewerAdded;

            Task.Run(() =>
            {
                Remotely_ScreenCast.Program.Main(new string[] { "-mode", "Normal", "-host", Config.Host });
            });
            //ScreenCasterProcess = new Process();
            //ScreenCasterProcess.StartInfo = psi;
            //ScreenCasterProcess.OutputDataReceived += IPC.OutputDataReceived;
            //ScreenCasterProcess.ErrorDataReceived += IPC.ErrorDataReceived;
            //ScreenCasterProcess.Exited += ScreenCasterProcess_Exited;
            //ScreenCasterProcess.Start();
        }

        private void ViewerAdded(object sender, Remotely_ScreenCast.Models.Viewer viewer)
        {
            Viewers.Add(new Viewer()
            {
                ConnectionId = viewer.ViewerConnectionID,
                HasControl = true,
                Name = viewer.Name
            });
        }

        private void ViewerRemoved(object sender, string viewerID)
        {
            var viewer = Viewers.FirstOrDefault(x => x.ConnectionId == viewerID);
            if (viewer != null)
            {
                Viewers.Remove(viewer);
            }
        }

        internal void CopyLink()
        {
            Clipboard.SetText($"{Host}/RemoteControl?sessionID={SessionID.Replace(" ", "")}");
        }

        public void PromptForHostName()
        {
            var prompt = new HostNamePrompt();
            if (!string.IsNullOrWhiteSpace(Config.Host))
            {
                HostNamePromptViewModel.Current.Host = Config.Host;
            }
            prompt.Owner = App.Current?.MainWindow;
            prompt.ShowDialog();
            var result = HostNamePromptViewModel.Current.Host.TrimEnd("/".ToCharArray());
            if (result != Config.Host)
            {
                Config.Host = result;
                Config.Save();
                FirePropertyChanged("Host");
            }
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

        private void ScreenCasterProcess_Exited(object sender, EventArgs e)
        {
            MessageBox.Show("The screen sharing process has stopped unexpectedly.  Remotely will now close.", "Sharing Stopped", MessageBoxButton.OK, MessageBoxImage.Warning);
            App.Current.Shutdown();
        }

        private void SessionIDChanged(object sender, string sessionID)
        {
            var formattedSessionID = "";
            for (var i = 0; i < sessionID.Length; i += 3)
            {
                formattedSessionID += sessionID.Substring(i, 3) + " ";
            }
            SessionID = formattedSessionID.Trim();
        }
    }
}
