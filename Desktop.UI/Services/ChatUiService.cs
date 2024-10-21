using Remotely.Desktop.Shared.Abstractions;
using System.ComponentModel;
using Remotely.Desktop.UI.Controls.Dialogs;
using Remotely.Shared.Models;

namespace Remotely.Desktop.UI.Services;

public class ChatUiService : IChatUiService
{
    private readonly IUiDispatcher _dispatcher;
    private readonly IDialogProvider _dialogProvider;
    private readonly IViewModelFactory _viewModelFactory;
    private IChatWindowViewModel? _chatViewModel;

    public ChatUiService(
        IUiDispatcher dispatcher,
        IDialogProvider dialogProvider,
        IViewModelFactory viewModelFactory)
    {
        _dispatcher = dispatcher;
        _dialogProvider = dialogProvider;
        _viewModelFactory = viewModelFactory;
    }

    public event EventHandler? ChatWindowClosed;

    public async Task ReceiveChat(ChatMessage chatMessage)
    {
        await _dispatcher.InvokeAsync(async () =>
        {
            if (chatMessage.Disconnected)
            {
                await _dialogProvider.Show("Your partner has disconnected from the chat.", "Partner Disconnected", MessageBoxType.OK);
                Environment.Exit(0);
                return;
            }

            if (_chatViewModel != null)
            {
                _chatViewModel.SenderName = chatMessage.SenderName;
                _chatViewModel.ChatMessages.Add(chatMessage);
            }
        });
    }

    public void ShowChatWindow(string organizationName, StreamWriter writer)
    {
        _dispatcher.Post(() =>
        {
            _chatViewModel = _viewModelFactory.CreateChatWindowViewModel(organizationName, writer);
            var chatWindow = new ChatWindow()
            {
                DataContext = _chatViewModel
            };

            chatWindow.Closing += ChatWindow_Closing;
            _dispatcher.ShowMainWindow(chatWindow);
        });
    }

    private void ChatWindow_Closing(object? sender, CancelEventArgs e)
    {
        ChatWindowClosed?.Invoke(this, e);
    }
}
