using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Remotely.Desktop.Linux.ViewModels;
using System;
using System.Threading.Tasks;

namespace Remotely.Desktop.Linux.Views
{
    public class ChatWindow : Window
    {
        public ChatWindow()
        {
            InitializeComponent();
        }

        private ChatWindowViewModel ViewModel => DataContext as ChatWindowViewModel;

        private void ChatWindow_Closed(object sender, System.EventArgs e)
        {
            Environment.Exit(0);
        }

        private async void ChatWindow_KeyUp(object sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                await ViewModel.SendChatMessage();
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            Closed += ChatWindow_Closed;
            Opened += ChatWindow_Opened;

            this.FindControl<Border>("TitleBanner").PointerPressed += TitleBanner_PointerPressed;

            this.FindControl<TextBox>("InputTextBox").KeyUp += ChatWindow_KeyUp;

            this.FindControl<ItemsControl>("MessagesListBox").ItemContainerGenerator.Materialized += ItemContainerGenerator_Materialized;
        }

        private void ChatWindow_Opened(object sender, EventArgs e)
        {
            Topmost = false;
        }

        private async void ItemContainerGenerator_Materialized(object sender, Avalonia.Controls.Generators.ItemContainerEventArgs e)
        {
            // Allows listbox height to adjust to content before scrolling the scrollviewer.
            await Task.Delay(1);
            // TODO: Replace with ScrollToEnd when implemented.
            var scrollViewer = this.FindControl<ScrollViewer>("MessagesScrollViewer");
            var listBox = this.FindControl<ItemsControl>("MessagesListBox");
            scrollViewer.Offset = new Vector(0, listBox.Bounds.Height);
        }


        private void TitleBanner_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == Avalonia.Input.PointerUpdateKind.LeftButtonPressed)
            {
                BeginMoveDrag(e);
            }
        }
    }
}
