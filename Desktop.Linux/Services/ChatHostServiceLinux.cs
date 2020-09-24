using Avalonia.Controls;
using Avalonia.Threading;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Linux.Controls;
using Remotely.Desktop.Linux.ViewModels;
using Remotely.Desktop.Linux.Views;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Desktop.Linux.Services
{
    public class ChatHostServiceLinux : IChatHostService
    {
        private ChatWindowViewModel ChatViewModel { get; set; }
        private NamedPipeServerStream NamedPipeStream { get; set; }
        private StreamReader Reader { get; set; }
        private StreamWriter Writer { get; set; }


        public async Task StartChat(string requesterID, string organizationName)
        {
            NamedPipeStream = new NamedPipeServerStream("Remotely_Chat" + requesterID, PipeDirection.InOut, 10, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            Writer = new StreamWriter(NamedPipeStream);
            Reader = new StreamReader(NamedPipeStream);

            var cts = new CancellationTokenSource(10000);
            try
            {
                await NamedPipeStream.WaitForConnectionAsync(cts.Token);
            }
            catch (TaskCanceledException)
            {
                Logger.Write("A chat session was attempted, but the client failed to connect in time.", Shared.Enums.EventType.Warning);
                Environment.Exit(0);
            }

            Dispatcher.UIThread.Post(() =>
            {
                var chatWindow = new ChatWindow();
                chatWindow.Closing += ChatWindow_Closing; ;
                ChatViewModel = chatWindow.DataContext as ChatWindowViewModel;
                ChatViewModel.PipeStreamWriter = Writer;
                ChatViewModel.OrganizationName = organizationName;
                App.Current.Run(chatWindow);
            });

            _ = Task.Run(ReadFromStream);
        }

        private void ChatWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                NamedPipeStream?.Disconnect();
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
                        await Dispatcher.UIThread.InvokeAsync(async () =>
                        {
                            if (chatMessage.Disconnected)
                            {
                                await MessageBox.Show("The partner has disconnected.", "Partner Disconnected", MessageBoxType.OK);
                                Environment.Exit(0);
                                return;
                            }
                            ChatViewModel.SenderName = chatMessage.SenderName;
                            ChatViewModel.ChatMessages.Add(chatMessage);
                        });
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
