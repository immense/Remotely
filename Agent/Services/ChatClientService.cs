using Microsoft.AspNetCore.SignalR.Client;
using Remotely.Shared.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    public class ChatClientService
    {
        public ChatClientService(AppLauncher appLauncher)
        {
            AppLauncher = appLauncher;
        }

        private SemaphoreSlim MessageLock { get; } = new SemaphoreSlim(1);
        private AppLauncher AppLauncher { get; }
        private CacheItemPolicy CacheItemPolicy { get; } = new CacheItemPolicy()
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
            RemovedCallback = new CacheEntryRemovedCallback(args =>
            {
                (args.CacheItem.Value as NamedPipeClientStream).Dispose();
            })
        };

        private MemoryCache ChatClients { get; } = new MemoryCache("ChatClients");
        public async Task SendMessage(string message, string senderConnectionID, HubConnection hubConnection)
        {
            try
            {
                if (await MessageLock.WaitAsync(30000))
                {
                    NamedPipeClientStream clientPipe;
                    if (!ChatClients.Contains(senderConnectionID))
                    {
                        var rcBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScreenCast", OSUtils.ScreenCastExecutableFileName);
                        await AppLauncher.LaunchChatService(senderConnectionID, hubConnection);

                        clientPipe = new NamedPipeClientStream(".", "Remotely_Chat" + senderConnectionID, PipeDirection.InOut, PipeOptions.Asynchronous);
                        clientPipe.Connect(15000);
                        if (!clientPipe.IsConnected)
                        {
                            Logger.Write("Failed to connect to chat host.");
                            return;
                        }
                        ChatClients.Add(senderConnectionID, clientPipe, CacheItemPolicy);
                    }

                    clientPipe = (NamedPipeClientStream)ChatClients.Get(senderConnectionID);

                    if (!clientPipe.IsConnected)
                    {
                        ChatClients.Remove(senderConnectionID);
                        await hubConnection.SendAsync("DisplayMessage", "Chat disconnected.  Please try again.", "Chat disconnected.");
                        return;
                    }

                    using (var sw = new StreamWriter(clientPipe, leaveOpen: true))
                    {
                        await sw.WriteLineAsync(message);
                        await sw.FlushAsync();
                    }

                    _ = Task.Run(async () => { await ReadFromStream(clientPipe, senderConnectionID, hubConnection); });
                }
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
            while (clientPipe.IsConnected)
            {
                using (var sr = new StreamReader(clientPipe, leaveOpen: true))
                {
                    var message = await sr.ReadLineAsync();
                    await hubConnection.SendAsync("Chat", message, senderConnectionID);
                }               
            }
        }
    }
}
