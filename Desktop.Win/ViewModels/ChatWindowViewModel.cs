using Remotely.Desktop.Core.ViewModels;
using Remotely.Shared.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Desktop.Win.ViewModels
{
    public class ChatWindowViewModel : ViewModelBase
    {
        private string inputText;
        private string organizationName = "your IT provider";
        private string senderName = "a technician";
        public ObservableCollection<ChatMessage> ChatMessages { get; } = new ObservableCollection<ChatMessage>();

        public string InputText
        {
            get
            {
                return inputText;
            }
            set
            {
                inputText = value;
                FirePropertyChanged(nameof(InputText));
            }
        }

        public string OrganizationName
        {
            get
            {
                return organizationName;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value) ||
                    value == organizationName)
                {
                    return;
                }


                organizationName = value;
                FirePropertyChanged(nameof(OrganizationName));
            }
        }

        public StreamWriter PipeStreamWriter { get; set; }
        public string SenderName
        {
            get
            {
                return senderName;
            }
            set
            {
                if (value == senderName)
                {
                    return;
                }

                senderName = value;
                FirePropertyChanged(nameof(SenderName));
            }
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
