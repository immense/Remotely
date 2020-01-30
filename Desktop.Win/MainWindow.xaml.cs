using Remotely.Desktop.Win.ViewModels;
using Remotely.ScreenCast.Core.Models;
using Remotely.ScreenCast.Core.Services;
using Remotely.Shared.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Remotely.Desktop.Win
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await MainWindowViewModel.Current.Init();
        }


        private async void CopyLinkButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.Current.CopyLink();
            var tooltip = new ToolTip();
            tooltip.PlacementTarget = sender as Button;
            tooltip.Placement = PlacementMode.Bottom;
            tooltip.VerticalOffset = 5;
            tooltip.Content = "Copied to clipboard!";
            tooltip.HasDropShadow = true;
            tooltip.StaysOpen = false;
            tooltip.IsOpen = true;

            await Task.Delay(750);
            var animation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(750));
            tooltip.BeginAnimation(OpacityProperty, animation);
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsOpen = true;
        }
    }
}
