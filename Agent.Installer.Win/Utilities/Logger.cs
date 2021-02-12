using System;
using System.IO;
using System.Linq;

namespace Remotely.Agent.Installer.Win.Utilities
{
    public class Logger
    {
        private static string LogPath => Path.Combine(Path.GetTempPath(), "Remotely_Installer.log");
        private static object WriteLock { get; } = new object();
        public static void Debug(string message)
        {
            lock (WriteLock)
            {
#if DEBUG
                CheckLogFileExists();

                File.AppendAllText(LogPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[Debug]\t{message}{Environment.NewLine}");

#endif
                System.Diagnostics.Debug.WriteLine(message);
            }

        }

        public static void Write(string message)
        {
            try
            {
                lock (WriteLock)
                {
                    CheckLogFileExists();
                    File.AppendAllText(LogPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[Info]\t{message}{Environment.NewLine}");
                    Console.WriteLine(message);
                }
            }
            catch { }
        }

        public static void Write(Exception ex)
        {
            lock (WriteLock)
            {
                try
                {
                    CheckLogFileExists();

                    var exception = ex;

                    while (exception != null)
                    {
                        File.AppendAllText(LogPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[Error]\t{exception?.Message}\t{exception?.StackTrace}\t{exception?.Source}{Environment.NewLine}");
                        Console.WriteLine(exception.Message);
                        exception = exception.InnerException;
                    }
                }
                catch { }
            }
        }

        private static void CheckLogFileExists()
        {
            if (!File.Exists(LogPath))
            {
                File.Create(LogPath).Close();
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
