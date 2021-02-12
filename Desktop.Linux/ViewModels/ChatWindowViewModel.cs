using Avalonia.Controls;
using ReactiveUI;
using Remotely.Desktop.Linux.Services;
using Remotely.Shared.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Remotely.Desktop.Linux.ViewModels
{
    public class ChatWindowViewModel : BrandedViewModelBase
    {
        private string _inputText;
        private string _organizationName = "your IT provider";
        private string _senderName = "a technician";
        public ObservableCollection<ChatMessage> ChatMessages { get; } = new ObservableCollection<ChatMessage>();

        public string ChatSessionHeader => $"Chat session with {OrganizationName}";

        public ICommand CloseCommand => new Executor((param) =>
        {
            (param as Window)?.Close();
        });
        public string InputText
        {
            get => _inputText;
            set => this.RaiseAndSetIfChanged(ref _inputText, value);
        }

        public ICommand MinimizeCommand => new Executor((param) =>
                {
                    (param as Window).WindowState = WindowState.Minimized;
                });

        public string OrganizationName
        {
            get => _organizationName;
            set
            {
                this.RaiseAndSetIfChanged(ref _organizationName, value);
                this.RaisePropertyChanged(nameof(ChatSessionHeader));
            }
        }

        public StreamWriter PipeStreamWriter { get; set; }

        public string SenderName
        {
            get => _senderName;
            set => this.RaiseAndSetIfChanged(ref _senderName, value);
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
