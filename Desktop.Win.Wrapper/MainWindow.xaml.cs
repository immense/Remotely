using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Remotely.Desktop.Win.Wrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string baseDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "Remotely_Desktop")).FullName;
        private readonly string currentVersionDir = Path.Combine(baseDir, "Current");
        private readonly string remotelyDesktopFilename = "Remotely_Desktop.exe";
        private readonly string tempDir = Directory.CreateDirectory(Path.Combine(baseDir, Guid.NewGuid().ToString())).FullName;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CleanupOldFiles()
        {
            foreach (var fse in Directory.GetFileSystemEntries(baseDir))
            {
                try
                {
                    if (Directory.Exists(fse) && fse != currentVersionDir)
                    {
                        Directory.Delete(fse, true);
                    }
                    else if (File.Exists(fse))
                    {
                        File.Delete(fse);
                    }
                }
                catch { }

            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExtractRemotely()
        {
            try
            {
                var zipPath = Path.Combine(tempDir, "Remotely_Desktop.zip");
                var tempExePath = Path.Combine(tempDir, remotelyDesktopFilename);

                using (var mrs = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Remotely.Desktop.Win.Wrapper.Remotely_Desktop.zip"))
                {
                    using (var fs = new FileStream(zipPath, FileMode.Create))
                    {
                        mrs.CopyTo(fs);
                    }
                }

                using (var zipArchive = ZipFile.OpenRead(zipPath))
                {
                    zipArchive.GetEntry(remotelyDesktopFilename).ExtractToFile(tempExePath);
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(tempExePath);

                    var targetExePath = Path.Combine(currentVersionDir, remotelyDesktopFilename);
                    if (File.Exists(targetExePath) &&
                        FileVersionInfo.GetVersionInfo(targetExePath).FileVersion == fileVersionInfo.FileVersion)
                    {
                        return;
                    }

                    try
                    {
                        var currentVersionDir = Path.GetDirectoryName(targetExePath);
                        if (Directory.Exists(currentVersionDir))
                        {
                            Directory.Delete(currentVersionDir, true);
                        }
                        Directory.CreateDirectory(currentVersionDir);
                        ZipFile.ExtractToDirectory(zipPath, currentVersionDir);
                    }
                    catch { }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while extracting files.  Error: " +
                    Environment.NewLine + Environment.NewLine + ex.Message, "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void StartRemotely()
        {
            try
            {
                Process.Start(Path.Combine(currentVersionDir, remotelyDesktopFilename));
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while starting Remotely.  Error: " +
                    Environment.NewLine + Environment.NewLine + ex.Message, "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Extracting files...";
            await Task.Run(ExtractRemotely);
            await Task.Run(StartRemotely);
            await Task.Run(CleanupOldFiles);
            await Task.Run(AddFirewallRule);
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void AddFirewallRule()
        {
            var psi = new ProcessStartInfo()
            {
                FileName = "netsh",
                Arguments = "advfirewall firewall delete rule name=\"Remotely Desktop\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };

            Process.Start(psi).WaitForExit();

            psi.Arguments = $"advfirewall firewall add rule name=\"Remotely Desktop\" program=\"{Path.Combine(currentVersionDir, remotelyDesktopFilename)}\" protocol=any dir=in enable=yes action=allow description=\"The agent that allows screen sharing and remote control for Remotely.\"";

            Process.Start(psi).WaitForExit();
        }
    }
}
