using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Services;

public interface IFileLogsManager
{
    Task<bool> AnyLogsExist();
    Task DeleteLogs();
    IAsyncEnumerable<byte[]> ReadAllBytes();
}

public class FileLogsManager : IFileLogsManager
{
    public async Task<bool> AnyLogsExist()
    {
        using var logLock = await FileLoggerDefaults.AcquireLock();

        var componentName = Assembly.GetExecutingAssembly().GetName().Name;
        var directory = Path.Combine(FileLoggerDefaults.LogsFolderPath, $"{componentName}");

        if (Directory.Exists(directory))
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (new FileInfo(file).Length > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public async Task DeleteLogs()
    {
        using var logLock = await FileLoggerDefaults.AcquireLock();

        var componentName = Assembly.GetExecutingAssembly().GetName().Name;
        var directory = Path.Combine(FileLoggerDefaults.LogsFolderPath, $"{componentName}");

        if (Directory.Exists(directory))
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }
    }

    public async IAsyncEnumerable<byte[]> ReadAllBytes()
    {
        using var logLock = await FileLoggerDefaults.AcquireLock();

        var componentName = Assembly.GetExecutingAssembly().GetName().Name;
        var directory = Path.Combine(FileLoggerDefaults.LogsFolderPath, $"{componentName}");

        if (!Directory.Exists(directory))
        {
            yield break;
        }

        var files = Directory
                .GetFiles(directory)
                .OrderBy(File.GetCreationTime);

        foreach (var file in files)
        {
            foreach (var chunk in File.ReadAllBytes(file).Chunk(50_000))
            {
                yield return File.ReadAllBytes(file);
            }
        }
    }
}
