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
        public ChatClientService(IAppLauncher appLauncher)
        {
            AppLauncher = appLauncher;
        }

        private SemaphoreSlim MessageLock { get; } = new(1,1);
        private IAppLauncher AppLauncher { get; }
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

        private MemoryCache ChatClients { get; } = new("ChatClients");

        public async Task SendMessage(string senderName,
            string message,
            string orgName,
            bool disconnected,
            string senderConnectionID,
            HubConnection hubConnection)
        {
            if (!await MessageLock.WaitAsync(30000))
            {
                Logger.Write("Timed out waiting for chat message lock.", Shared.Enums.EventType.Warning);
                return;
            }

            try
            {
                ChatSession chatSession;
                if (!ChatClients.Contains(senderConnectionID))
                {
                    if (disconnected)
                    {
                        // Don't start a new session just to show a disconnected message.
                        return;
                    }

                    var procID = await AppLauncher.LaunchChatService(orgName, senderConnectionID, hubConnection);

                    if (procID > 0)
                    {
                        Logger.Write($"Chat app started.  Process ID: {procID}");
                    }
                    else
                    {
                        Logger.Write($"Chat app did not start successfully.");
                        return;
                    }

                    var clientPipe = new NamedPipeClientStream(".", "Remotely_Chat" + senderConnectionID, PipeDirection.InOut, PipeOptions.Asynchronous);
                    clientPipe.Connect(15000);
                    if (!clientPipe.IsConnected)
                    {
                        Logger.Write("Failed to connect to chat host.");
                        return;
                    }
                    chatSession = new ChatSession() { PipeStream = clientPipe, ProcessID = procID };
                    _ = Task.Run(async () => { await ReadFromStream(chatSession.PipeStream, senderConnectionID, hubConnection); });
                    ChatClients.Add(senderConnectionID, chatSession, CacheItemPolicy);
                }

                chatSession = (ChatSession)ChatClients.Get(senderConnectionID);

                if (!chatSession.PipeStream.IsConnected)
                {
                    ChatClients.Remove(senderConnectionID);
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
                MessageLock.Release();
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
            ChatClients.Remove(senderConnectionID);
        }
    }
}
