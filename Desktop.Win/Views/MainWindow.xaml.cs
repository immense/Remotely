using Remotely.Desktop.Win.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Remotely.Desktop.Win.Views
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

        public MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void CopyLinkButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CopyLink();
            var tooltip = new ToolTip
            {
                PlacementTarget = sender as Button,
                Placement = PlacementMode.Bottom,
                VerticalOffset = 5,
                Content = "Copied to clipboard!",
                HasDropShadow = true,
                StaysOpen = false,
                IsOpen = true
            };

            await Task.Delay(750);
            var animation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(750));
            tooltip.BeginAnimation(OpacityProperty, animation);
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ViewModel?.ShutdownApp();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this) &&
                ViewModel != null)
            {
                await ViewModel?.Init();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
