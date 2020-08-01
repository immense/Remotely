using Avalonia.Controls;
using ReactiveUI;
using Remotely.Desktop.Linux.Services;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Remotely.Desktop.Linux.ViewModels
{
    public class ChatWindowViewModel : ViewModelBase
    {
        private string inputText;
        private string organizationName = "your IT provider";
        private string senderName = "a technician";
        public ObservableCollection<ChatMessage> ChatMessages { get; } = new ObservableCollection<ChatMessage>();

        public string ChatSessionHeader => $"Chat session with {OrganizationName}";

        public ICommand CloseCommand => new Executor((param) =>
        {
            (param as Window)?.Close();
        });
        public string InputText
        {
            get => inputText;
            set => this.RaiseAndSetIfChanged(ref inputText, value);
        }

        public ICommand MinimizeCommand => new Executor((param) =>
                {
                    (param as Window).WindowState = WindowState.Minimized;
                });

        public string OrganizationName
        {
            get => organizationName;
            set 
            {
                this.RaiseAndSetIfChanged(ref organizationName, value);
                this.RaisePropertyChanged(nameof(ChatSessionHeader));
            }
        }

        public StreamWriter PipeStreamWriter { get; set; }

        public string SenderName
        {
            get => senderName;
            set => this.RaiseAndSetIfChanged(ref senderName, value);
        }

        public async Task SendChatMessage()
        {
            if (string.IsNullOrWhiteSpace(InputText))
            {
                return;
            }

            var chatMessage = new ChatMessage(string.Empty, InputText);
            InputText = string.Empty;
            await PipeStreamWriter.WriteLineAsync(JsonSerializer.Serialize(chatMessage));
            await PipeStreamWriter.FlushAsync();
            chatMessage.SenderName = "You";
            ChatMessages.Add(chatMessage);
        }
    }
}
