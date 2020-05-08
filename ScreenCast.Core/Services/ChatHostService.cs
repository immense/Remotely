using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.ScreenCast.Core.Services
{
    public class ChatHostService
    {

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

        private int MaxCursorTop { get; set; }
        private NamedPipeServerStream NamedPipeStream { get; set; }
        private StreamReader Reader { get; set; }
        private StreamWriter Writer { get; set; }
        public async Task StartChat(string requesterID, string organizationName)
        {
            NamedPipeStream = new NamedPipeServerStream("Remotely_Chat" + requesterID, PipeDirection.InOut, 10, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            Writer = new StreamWriter(NamedPipeStream);
            Reader = new StreamReader(NamedPipeStream);
            
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Title = "Remotely Chat";
            if (string.IsNullOrWhiteSpace(organizationName))
            {
                Console.Write(AsciiLogo);
            }
            else
            {
                WriteOrgnanizationName(organizationName);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Your IT administrator would like to chat!");
            Console.WriteLine();
            Console.WriteLine("Connecting...");
            Console.WriteLine();

            var cts = new CancellationTokenSource(10000);
            try
            {
                await NamedPipeStream.WaitForConnectionAsync(cts.Token);
            }
            catch (TaskCanceledException)
            {
                await Close();
                return;
            }
            _ = Task.Run(ReadFromStream);
            Console.WriteLine("You're now connected with a technician.");
            Console.WriteLine();
            Console.WriteLine("Type your responses below and hit Enter to send.");
            Console.WriteLine("Press Ctrl + C to exit.");

            while (NamedPipeStream.IsConnected)
            {
                SetPrompt();
                var message = Console.ReadLine();
                await Writer.WriteLineAsync(message);
                await Writer.FlushAsync();
            }

            await Close();
        }

        private async Task Close()
        {
            Console.WriteLine("Connection failed.  Closing...");
            await Task.Delay(3000);
            Environment.Exit(0);
        }

        private async Task ReadFromStream()
        {
            while (NamedPipeStream.IsConnected)
            {
                var message = await Reader.ReadLineAsync();
                Console.WriteLine();
                Console.WriteLine();
                var split = message.Split(":", 2);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{split[0]}: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(split[1]);
                SetPrompt();
            }
        }
        private void SetPrompt()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("You: ");
            Console.ForegroundColor = ConsoleColor.White;
        }
        private void WriteOrgnanizationName(string organizationName)
        {
            Console.WriteLine();
            Console.WriteLine();

            Console.Write("     ");
            for (var i = 0; i < organizationName.Length + 10; i++)
            {
                Console.Write("/");
            }
            Console.WriteLine();


            Console.Write("    ///");
            for (var i = 0; i < organizationName.Length + 4; i++)
            {
                Console.Write(" ");
            }
            Console.Write("///");
            Console.WriteLine();


            Console.WriteLine($"   ///  {organizationName}  ///");


            Console.Write("  ///");
            for (var i = 0; i < organizationName.Length + 4; i++)
            {
                Console.Write(" ");
            }
            Console.Write("///");
            Console.WriteLine();


            Console.Write(" ");
            for (var i = 0; i < organizationName.Length + 10; i++)
            {
                Console.Write("/");
            }
            Console.WriteLine();


            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
