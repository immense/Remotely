using Remotely.ScreenCast.Core.Communication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.ScreenCast.Core.Services
{
    public class ChatService
    {

        private NamedPipeServerStream NamedPipeStream { get; set; }
        private string AsciiLogo => @"  
  _____                      _       _       
 |  __ \                    | |     | |      
 | |__) |___ _ __ ___   ___ | |_ ___| |_   _ 
 |  _  // _ \ '_ ` _ \ / _ \| __/ _ \ | | | |
 | | \ \  __/ | | | | | (_) | ||  __/ | |_| |
 |_|  \_\___|_| |_| |_|\___/ \__\___|_|\__, |
                                        __/ |
                                       |___/ 
";

        public async Task StartChat()
        {
            NamedPipeStream = new NamedPipeServerStream("Remotely_Chat" + Process.GetCurrentProcess().Id, PipeDirection.InOut, 1);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Title = "Remotely Chat";
            Console.Write(AsciiLogo);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Your IT administrator would like to chat!");
            Console.WriteLine();
            Console.WriteLine("Connecting...");
            Console.WriteLine();

            // Cancellation token doesn't work.
            _ = Task.Run(async () => {
                await Task.Delay(10000);
                if (!NamedPipeStream.IsConnected)
                {
                    Console.WriteLine("Connection failed.  Closing...");
                    await Task.Delay(3000);
                    Environment.Exit(0);
                }
            });
            var cts = new CancellationTokenSource(10000);
            await NamedPipeStream.WaitForConnectionAsync(cts.Token);
            Console.WriteLine("You're now connected with a technician.");
            Console.WriteLine();
            Console.WriteLine("Type your responses below and hit Enter to send.");
            Console.WriteLine("Press Ctrl + C to exit.");
            Console.WriteLine();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("You: ");
                Console.ForegroundColor = ConsoleColor.White;
                var input = Console.ReadLine();
                await NamedPipeStream.WriteAsync(Encoding.UTF8.GetBytes(input));
            }
        }
    }
}
