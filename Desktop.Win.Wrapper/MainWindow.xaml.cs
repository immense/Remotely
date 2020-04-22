using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Remotely.Desktop.Win.Wrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExtractRemotely();
            ExtractInstallScript();
            RunInstallScript();
            RunRemotely();
        }

        private void RunRemotely()
        {
            throw new NotImplementedException();
        }

        private void ExtractRemotely()
        {
            throw new NotImplementedException();
        }

        private void RunInstallScript()
        {
            throw new NotImplementedException();
        }

        private void ExtractInstallScript()
        {
            throw new NotImplementedException();
        }
    }
}
