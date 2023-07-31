using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Utilities;

public static class ConsoleHelper
{
    public static string GetSelection(string promptMessage, params string[] options)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{promptMessage.Trim()}");
        Console.ForegroundColor = ConsoleColor.Gray;

        Console.WriteLine();

        for (var i = 0; i < options.Length; i++)
        {
            Console.WriteLine($"    [{i}] - {options[i]}");
        }

        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("Enter Response: ");
        Console.ForegroundColor = ConsoleColor.Gray;

        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    public static string ReadLine(string prompt, ConsoleColor promptColor = ConsoleColor.Cyan, string subprompt = "")
    {
        Console.ForegroundColor = promptColor;
        Console.WriteLine($"{prompt.Trim()}");
        Console.ForegroundColor = ConsoleColor.Gray;

        if (!string.IsNullOrWhiteSpace(subprompt))
        {
            Console.WriteLine(subprompt);
        }

        Console.WriteLine();

        Console.ForegroundColor = promptColor;
        Console.Write("Enter Response: ");
        Console.ForegroundColor = ConsoleColor.Gray;
        var response = Console.ReadLine();
        Console.WriteLine();

        return response ?? string.Empty;
    }

    public static bool TryParseBoolLike(string value, out bool result)
    {
        result = false;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (bool.TryParse(value.Trim(), out result))
        {
            return true;
        }

        if (value.Contains("yes", StringComparison.OrdinalIgnoreCase))
        {
            result = true;
            return true;
        }

        if (value.Contains("no", StringComparison.OrdinalIgnoreCase))
        {
            result = false;
            return true;
        }

        return false;
    }
    public static void WriteError(string message, int extraEmptyLines = 0, [CallerMemberName] string callerName = "")
    {
        WriteLine(message, extraEmptyLines, ConsoleColor.Red, callerName);
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
}
