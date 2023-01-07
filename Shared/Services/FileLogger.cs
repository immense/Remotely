using Microsoft.Extensions.Logging;
using Remotely.Shared.Extensions;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Shared.Services
{
    public class FileLogger : ILogger
    {
        private static readonly ConcurrentQueue<string> _logQueue = new();
        private static readonly ConcurrentStack<string> _scopeStack = new();
        private static readonly SemaphoreSlim _writeLock = new(1, 1);
        private static string _logDir;
        private readonly string _applicationName;
        private readonly string _categoryName;
        private readonly System.Timers.Timer _sinkTimer = new(5000) { AutoReset = false };

        public FileLogger(string applicationName, string categoryName)
        {
            _applicationName = applicationName?.SanitizeFileName() ?? string.Empty;
            _categoryName = categoryName;
            _sinkTimer.Elapsed += SinkTimer_Elapsed;
        }

        private string LogDir
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_logDir))
                {
                    return _logDir;
                }

                if (OperatingSystem.IsWindows())
                {
                    _logDir = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Remotely", "Logs")).FullName;
                }
                else
                {
                    _logDir = Directory.CreateDirectory("/var/log/remotely").FullName;
                }
                return _logDir;
            }
        }
        private string LogPath => Path.Combine(LogDir, $"LogFile_{_applicationName}_{DateTime.Now:yyyy-MM-dd}.log");

        public IDisposable BeginScope<TState>(TState state)
        {
            _scopeStack.Push($"{state}");
            return new NoopDisposable();
        }

        public void DeleteLogs()
        {
            try
            {
                _writeLock.Wait();

                if (File.Exists(LogPath))
                {
                    File.Delete(LogPath);
                }
            }
            catch { }
            finally
            {
                _writeLock.Release();
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace or LogLevel.Debug => EnvironmentHelper.IsDebug,
                LogLevel.Information or LogLevel.Warning or LogLevel.Error or LogLevel.Critical => true,
                _ => false,
            };
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            try
            {
                var scopeStack = _scopeStack.Any() ?
                    new string[] { _scopeStack.First(), _scopeStack.Last() } :
                    Array.Empty<string>();

                var message = FormatLogEntry(logLevel, _categoryName, $"{state}", exception, scopeStack);
                _logQueue.Enqueue(message);
                _sinkTimer.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error queueing log entry: {ex.Message}");
            }
        }

        public async Task<byte[]> ReadAllBytes()
        {
            try
            {
                _writeLock.Wait();

                CheckLogFileExists();

                return await File.ReadAllBytesAsync(LogPath);
            }
            catch (Exception ex)
            {
                this.LogError(ex, "Error while reading all bytes from logs.");
                return Array.Empty<byte>();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private void CheckLogFileExists()
        {
            _ = Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);

            if (!File.Exists(LogPath))
            {
                File.Create(LogPath).Close();

                try
                {
                    if (OperatingSystem.IsWindows())
                    {
                        Process.Start("cmd", $"/c icacls \"{LogPath}\" /grant Users:M").WaitForExit(1_000);
                    }
                    else if (OperatingSystem.IsLinux())
                    {
                        Process.Start("sudo", $"chmod 775 {LogPath}").WaitForExit(1_000);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error modifying log file permissions: {ex.Message}");
                }
            }

            if (File.Exists(LogPath))
            {
                var fi = new FileInfo(LogPath);
                while (fi.Length > 1_000_000)
                {
                    var content = File.ReadAllLines(LogPath);
                    File.WriteAllLines(LogPath, content.Skip(10));
                    fi = new FileInfo(LogPath);
                }
            }
        }

        private string FormatLogEntry(LogLevel logLevel, string categoryName, string state, Exception exception, string[] scopeStack)
        {
            var ex = exception;
            var exMessage = exception?.Message;

            while (ex?.InnerException is not null)
            {
                exMessage += $" | {ex.InnerException.Message}";
                ex = ex.InnerException;
            }

            var entry =
                $"[{logLevel}]\t" +
                $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}\t";

            entry += scopeStack.Any() ?
                        $"[{string.Join(" - ", scopeStack)} - {categoryName}]\t" :
                        $"[{categoryName}]\t";

            entry += $"Message: {state}\t";

            if (!string.IsNullOrWhiteSpace(exMessage))
            {
                entry += exMessage;
            }

            if (exception is not null)
            {
                entry += $"{Environment.NewLine}{exception.StackTrace}";
            }

            entry += Environment.NewLine;

            return entry;
        }

        private async void SinkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                await _writeLock.WaitAsync();

                CheckLogFileExists();

                var message = string.Empty;

                while (_logQueue.TryDequeue(out var entry))
                {
                    message += entry;
                }

                File.AppendAllText(LogPath, message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing log entry: {ex.Message}");
            }
            finally
            {
                _writeLock.Release();
            }
        }
        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
                _scopeStack.TryPop(out _);
            }
        }
    }
}
