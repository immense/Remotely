using Immense.RemoteControl.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Agent.Interfaces;
using Remotely.Agent.Models;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Caching;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    public class ChatClientService
    {
        private readonly IAppLauncher _appLauncher;
        private readonly MemoryCache _chatClients = new("ChatClients");
        private readonly SemaphoreSlim _messageLock = new(1,1);

        public ChatClientService(IAppLauncher appLauncher)
        {
            _appLauncher = appLauncher;
        }

        private CacheItemPolicy CacheItemPolicy { get; } = new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
            RemovedCallback = new CacheEntryRemovedCallback(args =>
            {
                var chatSession = (args.CacheItem.Value as ChatSession);
                chatSession.PipeStream.Dispose();
                Process.GetProcessById(chatSession.ProcessID)?.Kill();
            })
        };


        public async Task SendMessage(
            string senderName,
            string message,
            string orgName,
            string orgId,
            bool disconnected,
            string senderConnectionID,
            HubConnection hubConnection)
        {
            if (!await _messageLock.WaitAsync(30000))
            {
                Logger.Write("Timed out waiting for chat message lock.", Shared.Enums.EventType.Warning);
                return;
            }

            try
            {
                ChatSession chatSession;
                if (!_chatClients.Contains(senderConnectionID))
                {
                    if (disconnected)
                    {
                        // Don't start a new session just to show a disconnected message.
                        return;
                    }

                    var pipeName = Guid.NewGuid().ToString();
                    var procID = await _appLauncher.LaunchChatService(pipeName, senderConnectionID, senderName, orgName, orgId, hubConnection);

                    if (procID > 0)
                    {
                        Logger.Write($"Chat app started.  Process ID: {procID}");
                    }
                    else
                    {
                        Logger.Write($"Chat app did not start successfully.");
                        return;
                    }

                    var clientPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                    clientPipe.Connect(15000);
                    if (!clientPipe.IsConnected)
                    {
                        Logger.Write("Failed to connect to chat host.");
                        return;
                    }
                    chatSession = new ChatSession() { PipeStream = clientPipe, ProcessID = procID };
                    _ = Task.Run(async () => { await ReadFromStream(chatSession.PipeStream, senderConnectionID, hubConnection); });
                    _chatClients.Add(senderConnectionID, chatSession, CacheItemPolicy);
                }

                chatSession = (ChatSession)_chatClients.Get(senderConnectionID);

                if (!chatSession.PipeStream.IsConnected)
                {
                    _chatClients.Remove(senderConnectionID);
                    await hubConnection.SendAsync("DisplayMessage", "Chat disconnected.  Please try again.", "Chat disconnected.", "bg-warning", senderConnectionID);
                    return;
                }

                using var sw = new StreamWriter(chatSession.PipeStream, leaveOpen: true);
                var chatMessage = new ChatMessage(senderName, message, disconnected);
                await sw.WriteLineAsync(JsonSerializer.Serialize(chatMessage));
                await sw.FlushAsync();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                _messageLock.Release();
            }
        }

        private async Task ReadFromStream(NamedPipeClientStream clientPipe, string senderConnectionID, HubConnection hubConnection)
        {
            using var sr = new StreamReader(clientPipe, leaveOpen: true);
            while (clientPipe.IsConnected)
            {
                var messageJson = await sr.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(messageJson))
                {
                    var chatMessage = JsonSerializer.Deserialize<ChatMessage>(messageJson);
                    await hubConnection.SendAsync("Chat", chatMessage.Message, false, senderConnectionID);
                }
            }
            await hubConnection.SendAsync("Chat", string.Empty, true, senderConnectionID);
            _chatClients.Remove(senderConnectionID);
        }
    }
}
