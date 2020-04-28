using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PathIO = System.IO.Path;

namespace Remotely.Desktop.Win.Wrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string baseDir = Directory.CreateDirectory(PathIO.Combine(PathIO.GetTempPath(), "Remotely_Desktop")).FullName;
        private string tempDir;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Extracting files...";
            await Task.Run(CleanupOldFiles);
            tempDir = Directory.CreateDirectory(PathIO.Combine(baseDir, Guid.NewGuid().ToString())).FullName;
            await Task.Run(ExtractRemotely);
            await Task.Run(ExtractInstallScript);
            StatusText.Text = "Updating .NET Core runtime...";
            await Task.Run(RunInstallScript);
            Close();
        }

        private void CleanupOldFiles()
        {
            foreach (var fse in Directory.GetFileSystemEntries(baseDir))
            {
                try
                {
                    if (Directory.Exists(fse) && fse != tempDir)
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

        private void ExtractRemotely()
        {
            try
            {
                var zipPath = PathIO.Combine(tempDir, "Remotely_Desktop.zip");
                using (var mrs = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Remotely.Desktop.Win.Wrapper.Remotely_Desktop.zip"))
                {
                    using (var fs = new FileStream(zipPath, FileMode.Create))
                    {
                        mrs.CopyTo(fs);
                    }
                }
                ZipFile.ExtractToDirectory(zipPath, tempDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while extracting files.  Error: " +
                    Environment.NewLine + Environment.NewLine + ex.Message, "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void RunInstallScript()
        {
            try
            {
                var installPath = PathIO.Combine(tempDir, "Install.ps1");
                Process.Start(new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-executionpolicy bypass -f \"{installPath}\"",
                    WindowStyle = ProcessWindowStyle.Hidden
                }).WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while updating .NET Core.  Error: " +
                    Environment.NewLine + Environment.NewLine + ex.Message, "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExtractInstallScript()
        {
            try
            {
                var installPath = PathIO.Combine(tempDir, "Install.ps1");
                using (var mrs = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Remotely.Desktop.Win.Wrapper.Install.ps1"))
                {
                    using (var fs = new FileStream(installPath, FileMode.Create))
                    {
                        mrs.CopyTo(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while extracting files.  Error: " +
                    Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }
    }
}
