namespace Remotely.Shared.Models
{
    public class ChatMessage
    {
        public ChatMessage() { }

        public ChatMessage(string senderName, string message, bool disconnected = false)
        {
            SenderName = senderName;
            Message = message;
            Disconnected = disconnected;
        }

        public bool Disconnected { get; set; }
        public string Message { get; set; }
        public string SenderName { get; set; }
    }
}
