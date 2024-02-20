using Microsoft.Extensions.Logging;
using Remotely.Shared.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Agent.Services;

public interface IFileLogsManager
{
    Task<bool> AnyLogsExist(CancellationToken cancellationToken);
    Task DeleteLogs(CancellationToken cancellationToken);
    IAsyncEnumerable<byte[]> ReadAllBytes(CancellationToken cancellationToken);
}

public class FileLogsManager : IFileLogsManager
{
    public async Task<bool> AnyLogsExist(CancellationToken cancellationToken)
    {
        using var logLock = await FileLoggerDefaults.AcquireLock(cancellationToken);

        var componentName = Assembly.GetExecutingAssembly().GetName().Name;
        var directory = Path.Combine(FileLoggerDefaults.LogsFolderPath, $"{componentName}");

        if (Directory.Exists(directory))
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (new FileInfo(file).Length > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public async Task DeleteLogs(CancellationToken cancellationToken)
    {
        using var logLock = await FileLoggerDefaults.AcquireLock(cancellationToken);

        var componentName = Assembly.GetExecutingAssembly().GetName().Name;
        var directory = Path.Combine(FileLoggerDefaults.LogsFolderPath, $"{componentName}");

        if (Directory.Exists(directory))
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }
    }

    public async IAsyncEnumerable<byte[]> ReadAllBytes([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var logLock = await FileLoggerDefaults.AcquireLock(cancellationToken);

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
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
                yield return chunk;
            }
        }
    }
}
