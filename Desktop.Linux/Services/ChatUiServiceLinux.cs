using Avalonia.Controls;
using Avalonia.Threading;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Linux.Controls;
using Remotely.Desktop.Linux.ViewModels;
using Remotely.Desktop.Linux.Views;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Linux.Services
{
    public class ChatUiServiceLinux : IChatUiService
    {
        private ChatWindowViewModel ChatViewModel { get; set; }

        public event EventHandler ChatWindowClosed;

        public void ReceiveChat(ChatMessage chatMessage)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (chatMessage.Disconnected)
                {
                    await MessageBox.Show("The partner has disconnected.", "Partner Disconnected", MessageBoxType.OK);
                    Environment.Exit(0);
                    return;
                }

                if (ChatViewModel != null)
                {
                    ChatViewModel.SenderName = chatMessage.SenderName;
                    ChatViewModel.ChatMessages.Add(chatMessage);
                }
            });
        }

        public void ShowChatWindow(string organizationName, StreamWriter writer)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var chatWindow = new ChatWindow();
                chatWindow.Closing += ChatWindow_Closing;
                ChatViewModel = chatWindow.DataContext as ChatWindowViewModel;
                ChatViewModel.PipeStreamWriter = writer;
                ChatViewModel.OrganizationName = organizationName;
                App.Current.Run(chatWindow);
            });
        }

        private void ChatWindow_Closing(object sender, CancelEventArgs e)
        {
            ChatWindowClosed?.Invoke(this, null);
        }
    }
}
