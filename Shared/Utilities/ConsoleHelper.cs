using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Utilities
{
    public static class ConsoleHelper
    {
        public static string ReadLine(string prompt, ConsoleColor promptColor = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = promptColor;
            Console.Write($"{prompt.TrimEnd(':')}: ");
            Console.ForegroundColor = ConsoleColor.Gray;

            var response = Console.ReadLine();
            Console.WriteLine();

            return response;
        }

        public static void WriteLine(string message, int extraEmptyLines = 0, ConsoleColor foreground = ConsoleColor.Gray, string callerName = "")
        {
            if (!string.IsNullOrWhiteSpace(callerName))
            {
                message = $"[{callerName}] {message}";
            }
            Console.ForegroundColor = foreground;
            for (var i = 0; i < message.Length;)
            {
                var lineCount = 0;
                var trimLine = i > 0;

                var line = new string(message.Skip(i).TakeWhile(x => {
                    i++;
                    return lineCount++ < 60 || !char.IsWhiteSpace(x);
                }).ToArray());

                if (trimLine)
                {
                    line = line.Trim();
                }

                Console.WriteLine(line);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            for (var i = 0; i < extraEmptyLines; i++)
            {
                Console.WriteLine();
            }
        }

        public static void WriteError(string message, int extraEmptyLines = 0, [CallerMemberName] string callerName = "")
        {
            WriteLine(message, extraEmptyLines, ConsoleColor.Red, callerName);
        }
    }
}
