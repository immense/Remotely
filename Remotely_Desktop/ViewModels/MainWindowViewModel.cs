using Remotely_Desktop.Controls;
using Remotely_Desktop.Services;
using Remotely_ScreenCast;
using Remotely_ScreenCast.Capture;
using Remotely_ScreenCast.Models;
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

namespace Remotely_Desktop.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            Current = this;

            Program.SessionIDChanged += SessionIDChanged;
            Program.ViewerRemoved += ViewerRemoved;
            Program.ViewerAdded += ViewerAdded;
            Program.ScreenCastRequested += ScreenCastRequested;
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

        public string SessionID { get; set; }
        public ObservableCollection<Viewer> Viewers { get; } = new ObservableCollection<Viewer>();
        public void FirePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task Init()
        {
            SessionID = "Retrieving...";
            Config = Config.GetConfig();
            while (string.IsNullOrWhiteSpace(Config.Host))
            {
                Config.Host = "https://";
                PromptForHostName();
            }

            Program.ProcessArgs(new string[] { "-mode", "Normal", "-host", Config.Host });
            try
            {
               await Program.Connect();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                MessageBox.Show("Failed to connect to server.", "Connection Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Program.SetEventHandlers();

            await Task.Run(async () =>
            {
                await Program.HandleConnection();
            });
        }

        private void ScreenCastRequested(object sender, Tuple<string, string> args)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var result = MessageBox.Show($"You've received a connection request from {args.Item2}.  Accept?", "Connection Request", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Task.Run(() =>
                    {
                        ScreenCaster.BeginScreenCasting(args.Item1, args.Item2, Program.OutgoingMessages);
                    });
                }
            });
        }

        internal async Task RemoveViewers(IEnumerable<Viewer> viewerList)
        {
            foreach (Viewer viewer in viewerList)
            {
                viewer.DisconnectRequested = true;
                await Program.OutgoingMessages.SendViewerRemoved(viewer.ViewerConnectionID);
            }
        }

        private void ViewerAdded(object sender, Viewer viewer)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Viewers.Add(viewer);
            });
        }

        private void ViewerRemoved(object sender, string viewerID)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var viewer = Viewers.FirstOrDefault(x => x.ViewerConnectionID == viewerID);
                if (viewer != null)
                {
                    Viewers.Remove(viewer);
                }
            });
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
            if (!result.StartsWith("https://") && !result.StartsWith("http://"))
            {
                result = $"https://{result}";
            }
            if (result != Config.Host)
            {
                Config.Host = result;
                Config.Save();
                FirePropertyChanged("Host");
            }
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
