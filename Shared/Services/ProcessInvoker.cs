using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Utilities;

namespace Remotely.Shared.Services;

public interface IProcessInvoker
{
    string InvokeProcessOutput(string command, string arguments);
}

public class ProcessInvoker : IProcessInvoker
{
    private readonly ILogger<ProcessInvoker> _logger;

    public ProcessInvoker(ILogger<ProcessInvoker> logger)
    {
        _logger = logger;
    }

    public string InvokeProcessOutput(string command, string arguments)
    {
        try
        {
            var psi = new ProcessStartInfo(command, arguments)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "RunAs",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            var proc = Process.Start(psi);
            proc?.WaitForExit();

            return proc?.StandardOutput.ReadToEnd() ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start process.");
            return string.Empty;
        }
    }
}