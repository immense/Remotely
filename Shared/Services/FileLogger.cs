#nullable enable

using Microsoft.Extensions.Logging;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Shared.Services
{
    public class FileLogger : ILogger
    {
        private static readonly ConcurrentStack<string> _scopeStack = new();
        private static readonly SemaphoreSlim _writeLock = new(1, 1);
        private readonly string _categoryName;
        private readonly string _componentName;
        private readonly string _componentVersion;
        private DateTimeOffset _lastLogCleanup;

        public FileLogger(string componentName, string componentVersion, string categoryName)
        {
            _componentName = componentName;
            _componentVersion = componentVersion;
            _categoryName = categoryName;
        }

        private static string LogsFolderPath
        {
            get
            {
                if (OperatingSystem.IsWindows())
                {
                    var logsPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "Remotely",
                        "Logs");

                    if (EnvironmentHelper.IsDebug)
                    {
                        logsPath += "_Debug";
                    }
                    return logsPath;
                }

                if (OperatingSystem.IsLinux())
                {
                    if (EnvironmentHelper.IsDebug)
                    {
                        return "/var/log/remotely_debug";
                    }
                    return "/var/log/remotely";
                }

                throw new PlatformNotSupportedException();
            }
        }
        private string LogPath => Path.Combine(LogsFolderPath, _componentName, $"LogFile_{DateTime.Now:yyyy-MM-dd}.log");

        public IDisposable? BeginScope<TState>(TState state) 
            where TState : notnull
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

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _writeLock.Wait();

            try
            {

                var message = FormatLogEntry(logLevel, _categoryName, $"{state}", exception, _scopeStack.ToArray());
                CheckLogFileExists();
                File.AppendAllText(LogPath, message);
                CleanupLogs();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing log entry: {ex.Message}");
            }
            finally
            {
                _writeLock.Release();
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
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
            if (!File.Exists(LogPath))
            {
                File.Create(LogPath).Close();
            }
        }
        private void CleanupLogs()
        {
            if (DateTimeOffset.Now - _lastLogCleanup < TimeSpan.FromDays(1))
            {
                return;
            }

            _lastLogCleanup = DateTimeOffset.Now;

            var logFiles = Directory.GetFiles(Path.GetDirectoryName(LogPath)!)
                .Select(x => new FileInfo(x))
                .Where(x => DateTime.Now - x.CreationTime > TimeSpan.FromDays(7));

            foreach (var file in logFiles)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while trying to delete log file {file.FullName}.  Message: {ex.Message}");
                }
            }
        }
        private string FormatLogEntry(LogLevel logLevel, string categoryName, string state, Exception? exception, string[] scopeStack)
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
                $"[v{_componentVersion}]\t" +
                $"[Process ID: {Environment.ProcessId}]\t" +
                $"[Thread ID: {Environment.CurrentManagedThreadId}]\t" +
                $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}]\t";

            if (exception is not null)
            {
                entry += $"[Exception: {exception.GetType().Name}]\t";
            }

            entry += scopeStack.Any() ?
                        $"[{categoryName} => {string.Join(" => ", scopeStack)}]\t" :
                        $"[{categoryName}]\t";

            entry += $"{state}\t";

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

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
                _scopeStack.TryPop(out _);
            }
        }
    }
}
