using System;
using System.IO;
using System.Linq;

namespace Remotely.Agent.Installer.Win.Utilities
{
    public class Logger
    {
        private static readonly string _logDir = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Remotely", "Logs")).FullName;
        private static readonly string _logPath = Path.Combine(_logDir, "Installer.log");
        private static readonly object _writeLock = new object();

        public static void Debug(string message)
        {
            lock (_writeLock)
            {
#if DEBUG
                CheckLogFileExists();

                File.AppendAllText(_logPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[Debug]\t{message}{Environment.NewLine}");

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
                    File.AppendAllText(_logPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[Info]\t{message}{Environment.NewLine}");
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
                        File.AppendAllText(_logPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[Error]\t{exception?.Message}\t{exception?.StackTrace}\t{exception?.Source}{Environment.NewLine}");
                        Console.WriteLine(exception.Message);
                        exception = exception.InnerException;
                    }
                }
                catch { }
            }
        }

        private static void CheckLogFileExists()
        {
            if (!File.Exists(_logPath))
            {
                File.Create(_logPath).Close();
            }

            if (File.Exists(_logPath))
            {
                var fi = new FileInfo(_logPath);
                while (fi.Length > 1000000)
                {
                    var content = File.ReadAllLines(_logPath);
                    File.WriteAllLines(_logPath, content.Skip(10));
                    fi = new FileInfo(_logPath);
                }
            }
        }
    }
}
