using Remotely.Shared.Enums;
using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Shared.Utilities
{
    public static class Logger
    {
        private static string LogPath => Path.Combine(Path.GetTempPath(), "Remotely_Logs.log");
        private static SemaphoreSlim WriteLock { get; } = new(1, 1);
        public static void Debug(string message, [CallerMemberName] string callerName = "")
        {
            try
            {
                WriteLock.Wait();

                if (EnvironmentHelper.IsDebug)
                {
                    CheckLogFileExists();

                    File.AppendAllText(LogPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[Debug]\t[{callerName}]\t{message}{Environment.NewLine}");
                }

                System.Diagnostics.Debug.WriteLine(message);
            }
            catch { }
            finally
            {
                WriteLock.Release();
            }
        }

        public static async Task<byte[]> ReadAllLogs()
        {
            try
            {
                WriteLock.Wait();

                CheckLogFileExists();

                return await File.ReadAllBytesAsync(LogPath);
            }
            catch (Exception ex)
            {
                Write(ex);
                return Array.Empty<byte>();
            }
            finally
            {
                WriteLock.Release();
            }
        }

        public static void Write(string message, EventType eventType = EventType.Info, [CallerMemberName] string callerName = "")
        {
            try
            {
                WriteLock.Wait();

                CheckLogFileExists();
                File.AppendAllText(LogPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[{eventType}]\t[{callerName}]\t{message}{Environment.NewLine}");
                Console.WriteLine(message);
            }
            catch { }
            finally
            {
                WriteLock.Release();
            }
        }

        public static void Write(Exception ex, EventType eventType = EventType.Error, [CallerMemberName] string callerName = "")
        {
            try
            {
                WriteLock.Wait();

                CheckLogFileExists();

                var exception = ex;

                while (exception != null)
                {
                    File.AppendAllText(LogPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t[{eventType}]\t[{callerName}]\t{exception?.Message}\t{exception?.StackTrace}\t{exception?.Source}{Environment.NewLine}");
                    Console.WriteLine(exception.Message);
                    exception = exception.InnerException;
                }
            }
            catch { }
            finally
            {
                WriteLock.Release();
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
