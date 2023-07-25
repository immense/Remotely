using System;
using System.IO;
using System.Linq;

namespace Remotely.Agent.Installer.Win.Utilities;

public class Logger
{
    public static string LogsDir { get; } = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Remotely", "Logs")).FullName;
    public static string LogsPath { get; } = Path.Combine(LogsDir, "Installer.log");

    private static readonly object _writeLock = new object();

    public static void Debug(string message)
    {
        lock (_writeLock)
        {
#if DEBUG
            CheckLogFileExists();

            File.AppendAllText(LogsPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[Debug]\t{message}{Environment.NewLine}");

#endif
            System.Diagnostics.Debug.WriteLine(message);
        }

    }

    public static void Write(string message)
    {
        try
        {
            lock (_writeLock)
            {
                CheckLogFileExists();
                File.AppendAllText(LogsPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[Info]\t{message}{Environment.NewLine}");
                Console.WriteLine(message);
            }
        }
        catch { }
    }

    public static void Write(Exception ex)
    {
        lock (_writeLock)
        {
            try
            {
                CheckLogFileExists();

                var exception = ex;

                while (exception != null)
                {
                    File.AppendAllText(LogsPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[Error]\t{exception.Message}\t{exception.StackTrace}\t{exception.Source}{Environment.NewLine}");
                    Console.WriteLine(exception.Message);
                    exception = exception.InnerException;
                }
            }
            catch { }
        }
    }

    private static void CheckLogFileExists()
    {
        if (!File.Exists(LogsPath))
        {
            File.Create(LogsPath).Close();
        }

        if (File.Exists(LogsPath))
        {
            var fi = new FileInfo(LogsPath);
            while (fi.Length > 1000000)
            {
                var content = File.ReadAllLines(LogsPath);
                File.WriteAllLines(LogsPath, content.Skip(10));
                fi = new FileInfo(LogsPath);
            }
        }
    }
}
