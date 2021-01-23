using Remotely.Desktop.Core.Interfaces;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Services
{
    public class ChatClientService : IChatClientService
    {
        private readonly IChatUiService _chatUiService;

        public ChatClientService(IChatUiService chatUiService)
        {
            _chatUiService = chatUiService;
        }

        private NamedPipeClientStream NamedPipeStream { get; set; }
        private StreamReader Reader { get; set; }
        private StreamWriter Writer { get; set; }

        public async Task StartChat(string requesterID, string organizationName)
        {
            NamedPipeStream = new NamedPipeClientStream(".", "Remotely_Chat" + requesterID, PipeDirection.InOut, PipeOptions.Asynchronous);
            Writer = new StreamWriter(NamedPipeStream);
            Reader = new StreamReader(NamedPipeStream);

            
            try
            {
                await NamedPipeStream.ConnectAsync(15_000);
            }
            catch (TaskCanceledException)
            {
                Logger.Write("A chat session was attempted, but the client failed to connect in time.", Shared.Enums.EventType.Warning);
                Environment.Exit(0);
            }

            if (!NamedPipeStream.IsConnected)
            {
                Logger.Write("Failed to connect to chat host.");
                return;
            }

            _chatUiService.ChatWindowClosed += OnChatWindowClosed;

            _chatUiService.ShowChatWindow(organizationName, Writer);

            _ = Task.Run(ReadFromStream);
        }

        private void OnChatWindowClosed(object sender, EventArgs e)
        {
            try
            {
                NamedPipeStream?.Dispose();
            }
            catch { }
        }

        private async Task ReadFromStream()
        {
            while (NamedPipeStream.IsConnected)
            {
                try
                {
                    var messageJson = await Reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(messageJson))
                    {
                        var chatMessage = JsonSerializer.Deserialize<ChatMessage>(messageJson);
                        _chatUiService.ReceiveChat(chatMessage);
                       
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }
        }
    }
}
