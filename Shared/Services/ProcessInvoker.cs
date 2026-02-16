using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Remotely.Shared.Services;

public interface IProcessInvoker
{
    string InvokeProcessOutput(string command, string arguments, bool runAsAdmin = false);
}

public class ProcessInvoker : IProcessInvoker
{
    private readonly ILogger<ProcessInvoker> _logger;

    public ProcessInvoker(ILogger<ProcessInvoker> logger)
    {
        _logger = logger;
    }

    public string InvokeProcessOutput(string command, string arguments, bool runAsAdmin = false)
    {
        try
        {
            var psi = new ProcessStartInfo(command, arguments)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Only run as admin when explicitly requested
            if (runAsAdmin)
            {
                psi.Verb = "RunAs";
                psi.UseShellExecute = true;
                psi.RedirectStandardOutput = false;
                psi.RedirectStandardError = false;
            }

            var proc = Process.Start(psi);
            
            if (proc == null)
            {
                _logger.LogWarning("Failed to start process: {Command}", command);
                return string.Empty;
            }

            if (!runAsAdmin)
            {
                proc.WaitForExit();
                return proc.StandardOutput.ReadToEnd();
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start process.");
            return string.Empty;
        }
    }
}