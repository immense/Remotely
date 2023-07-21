using Immense.RemoteControl.Shared.Services;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Extensions;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Server.Services;

public interface ILogsManager
{
    Task DeleteLogs();

    IAsyncEnumerable<string> GetLogs(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        string messageFilter,
        LogLevel? logLevelFilter);

    string GetLogsDirectory();
    Task<FileInfo> ZipAllLogs();
}

public class LogsManager : ILogsManager
{
    private static readonly ReadOnlyDictionary<string, LogLevel> _logLevelMap = new(new Dictionary<string, LogLevel>()
    {
        ["[VRB]"] = LogLevel.Trace,
        ["[DBG]"] = LogLevel.Debug,
        ["[INF]"] = LogLevel.Information,
        ["[WRN]"] = LogLevel.Warning,
        ["[ERR]"] = LogLevel.Error,
        ["[FTL]"] = LogLevel.Critical
    });

    public static string DefaultLogsDirectory
    {
        get
        {
            var logsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (Directory.Exists("/remotely-data"))
            {
                logsDir = "/remotely-data/logs";
            }
            return logsDir;
        }
    }

    public async Task DeleteLogs()
    {
        var logsDir = GetLogsDirectory();

        var files = Directory.GetFiles(logsDir);

        if (!files.Any())
        {
            return;
        }

        await foreach (var file in files.ToAsyncEnumerable())
        {
            try
            {
                if (new FileInfo(file).LastWriteTime.Date == DateTime.Today)
                {
                    continue;
                }
                File.Delete(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete log file: {file}.  Message: {ex.Message}");
            }
        }
    }

    public async IAsyncEnumerable<string> GetLogs(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        string messageFilter,
        LogLevel? logLevelFilter)
    {
        var fromDate = startDate.UtcDateTime.Date;
        var toDate = endDate.UtcDateTime.Date.AddDays(1);

        var result = new StringBuilder();
        var logsDir = GetLogsDirectory();

        var files = Directory
            .GetFiles(logsDir)
            .Select(x => new FileInfo(x))
            .Where(x =>
                x.LastWriteTimeUtc >= fromDate &&
                x.LastWriteTimeUtc <= toDate)
            .OrderBy(x => x.LastWriteTimeUtc);

        foreach (var file in files)
        {
            var linesAsync = GetLines(file, messageFilter, logLevelFilter);
            await foreach (var line in linesAsync)
            {
                yield return line;
            }

        }
    }

    public string GetLogsDirectory()
    {
        return Directory.CreateDirectory(DefaultLogsDirectory).FullName;
    }

    public async Task<FileInfo> ZipAllLogs()
    {
        var logsDir = GetLogsDirectory();
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var tempDir = Directory.CreateDirectory(Path.Combine(baseDir, "temp", Guid.NewGuid().ToString())).FullName;
        var zipFilePath = Path.Combine(
            tempDir, 
            $"Remotely_Logs-{DateTimeOffset.Now:yyyy-MM-dd-HH-mm-ss}.zip");

        using var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Update);

        var files = Directory.GetFiles(logsDir);

        foreach (var file in files)
        {
            var entry = zipArchive.CreateEntry(Path.GetFileName(file));
            using var entryStream = entry.Open();
            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            await fs.CopyToAsync(entryStream);
        }

        return new FileInfo(zipFilePath);
    }
    private async IAsyncEnumerable<string> GetLines(
        FileInfo file,
        string messageFilter,
        LogLevel? logLevelFilter)
    {
        LogLevel? currentLogLevel = null;

        using var fs = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs);

        while (true)
        {
            var currentLine = await sr.ReadLineAsync();

            if (currentLine is null)
            {
                break;
            }

            if (logLevelFilter is not null)
            {
                if (TryGetLogLevel(currentLine, out var parsedLevel))
                {
                    currentLogLevel = parsedLevel;
                }

                if (currentLogLevel != logLevelFilter)
                {
                    continue;
                }
            }

            if (!string.IsNullOrWhiteSpace(messageFilter) && 
                !currentLine.Contains(messageFilter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            yield return currentLine;
        }
    }

    private bool TryGetLogLevel(
        string lineContent,
        [NotNullWhen(true)] out LogLevel? logLevel)
    {
        try
        {
            var logLevelTag = lineContent[31..36];
            if (_logLevelMap.TryGetValue(logLevelTag, out var result))
            {
                logLevel = result;
                return true;
            }
        }
        catch
        {
            // Ignored.
        }

        logLevel = default;
        return false;
    }
}
