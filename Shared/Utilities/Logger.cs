using Remotely.Shared.Enums;
using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Remotely.Shared.Utilities
{
    public static class Logger
    {
        private static string LogPath => Path.Combine(Path.GetTempPath(), "Remotely_Logs.log");
        private static object WriteLock { get; } = new object();
        public static void Debug(string message)
        {
            try
            {
                lock (WriteLock)
                {
                    if (EnvironmentHelper.IsDebug)
                    {
                        CheckLogFileExists();

                        File.AppendAllText(LogPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[Debug]\t{message}{Environment.NewLine}");
                    }

                    System.Diagnostics.Debug.WriteLine(message);
                }
            }
            catch { }
        }

        public static void Write(string message, EventType eventType = EventType.Info)
        {
            try
            {
                lock (WriteLock)
                {
                    CheckLogFileExists();
                    File.AppendAllText(LogPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[{eventType}]\t{message}{Environment.NewLine}");
                    Console.WriteLine(message);
                }
            }
            catch { }
        }

        public static void Write(Exception ex, EventType eventType = EventType.Error)
        {
            lock (WriteLock)
            {
                try
                {
                    CheckLogFileExists();

                    var exception = ex;

                    while (exception != null)
                    {
                        File.AppendAllText(LogPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[{eventType}]\t{exception?.Message}\t{exception?.StackTrace}\t{exception?.Source}{Environment.NewLine}");
                        Console.WriteLine(exception.Message);
                        exception = exception.InnerException;
                    }
                }
                catch { }
            }
        }

        public static void Write(Exception ex, string message, EventType eventType = EventType.Error)
        {
            Write(message, eventType);
            Write(ex, eventType);
        }

        private static void CheckLogFileExists()
        {
            if (!File.Exists(LogPath))
            {
                File.Create(LogPath).Close();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("sudo", $"chmod 777 {LogPath}").WaitForExit();
                }
            }
            if (File.Exists(LogPath))
            {
                var fi = new FileInfo(LogPath);
                while (fi.Length > 1000000)
                {
                    var content = File.ReadAllLines(LogPath);
                    File.WriteAllLines(LogPath, content.Skip(10));
                    fi = new FileInfo(LogPath);
                }
            }
        }
    }
}
