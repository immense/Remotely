using Remotely.Shared.Extensions;
using Serilog;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public interface ILogsManager
    {
        string GetLogsDirectory();
        Task<FileInfo> ZipAllLogs();
        Task DeleteLogs();
    }

    public class LogsManager : ILogsManager
    {
        public static LogsManager Default { get; } = new();

        public string GetLogsDirectory()
        {
            var logsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (Directory.Exists("/remotely-data"))
            {
                logsDir = "/remotely-data/logs";
            }
            return Directory.CreateDirectory(logsDir).FullName;
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
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to delete log file: {filename}.  Message: {exMessage}", file, ex.Message);
                }
            }
        }
    }
}
