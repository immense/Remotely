using Remotely.Agent.Installer.Win.Utilities;
using Remotely.Agent.Installer.Win.ViewModels;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Remotely.Agent.Installer.Win
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (CommandLineParser.CommandLineArgs.ContainsKey("quiet"))
            {
                Hide();
                ShowInTaskbar = false;
                _ = new MainWindowViewModel().Init();
            }
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await (DataContext as MainWindowViewModel).Init();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ShowServerUrlHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "This is the URL of the Remotely server that you're hosting.  The device will connect to this URL.", 
                "Server URL", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }

        private void ShowOrganizationIdHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "This is your organization ID on the Remotely server.  Since Remotely supports multi-tenancy, " +
                "this ID needs to be provided to determine who should have access." 
                + Environment.NewLine + Environment.NewLine +
                "You can find this ID on the Organization tab on the web app.", 
                "Organization ID", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }
        private void ShowSupportShortcutHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("If selected, the installer will create a desktop shortcut to the Get Support page for this device.", 
                "Support Shortcut",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
