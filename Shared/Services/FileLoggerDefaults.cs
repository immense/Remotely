using Immense.RemoteControl.Shared.Primitives;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Shared.Services;
public static class FileLoggerDefaults
{
    private static readonly SemaphoreSlim _logLock = new(1, 1);

    public static string LogsFolderPath
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

    public static async Task<IDisposable> AcquireLock(CancellationToken cancellationToken = default)
    {
        await _logLock.WaitAsync(cancellationToken);
        return new CallbackDisposable(() => _logLock.Release());
    }

    public static string GetLogPath(string componentName) => 
        Path.Combine(LogsFolderPath, componentName, $"LogFile_{DateTime.Now:yyyy-MM-dd}.log");

}
