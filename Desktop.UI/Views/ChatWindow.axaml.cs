using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Immense.RemoteControl.Desktop.UI.Views;

public partial class ChatWindow : Window
{
    public ChatWindow()
    {
        InitializeComponent();

        Closed += ChatWindow_Closed;
        Opened += ChatWindow_Opened;

        InputTextBox.KeyUp += ChatWindow_KeyUp;
        MessagesListBox.Loaded += MessageListBox_Loaded;
    }

    private void ChatWindow_Closed(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }

    private async void ChatWindow_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Enter &&
            DataContext is ChatWindowViewModel viewModel)
        {
            await viewModel.SendChatMessage();
        }
    }


    private void ChatWindow_Opened(object? sender, EventArgs e)
    {
        Topmost = false;
    }

    private async void MessageListBox_Loaded(object? sender, RoutedEventArgs e)
    {
        // Allows listbox height to adjust to content before scrolling the scrollviewer.
        await Task.Delay(1);
        MessagesScrollViewer.ScrollToEnd();
    }
}
