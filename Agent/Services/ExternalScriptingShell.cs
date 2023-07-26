using Immense.RemoteControl.Shared.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Remotely.Agent.Services;

public interface IExternalScriptingShell
{
    Process? ShellProcess { get; }
    Task Init(ScriptingShell shell, string shellProcessName, string lineEnding, string connectionId);
    Task<ScriptResult> WriteInput(string input, TimeSpan timeout);
}

public class ExternalScriptingShell : IExternalScriptingShell
{
    private static readonly ConcurrentDictionary<string, IExternalScriptingShell> _sessions = new();
    private readonly IConfigService _configService;
    private readonly ILogger<ExternalScriptingShell> _logger;
    private readonly ManualResetEvent _outputDone = new(false);
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private string _errorOut = string.Empty;
    private string _lastInputID = string.Empty;
    private string _lineEnding = Environment.NewLine;
    private System.Timers.Timer _processIdleTimeout = new(TimeSpan.FromMinutes(10))
    {
        AutoReset = false
    };

    private string? _senderConnectionId;
    private ScriptingShell _shell;
    private string _standardOut = string.Empty;

    public ExternalScriptingShell(
        IConfigService configService,
        ILogger<ExternalScriptingShell> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    public Process? ShellProcess { get; private set; }


    // TODO: Turn into cache and factory.
    public static async Task<IExternalScriptingShell> GetCurrent(ScriptingShell shell, string senderConnectionId)
    {
        if (_sessions.TryGetValue($"{shell}-{senderConnectionId}", out var session) &&
            session.ShellProcess?.HasExited != true)
        {
            return session;
        }
        else
        {
            session = Program.Services.GetRequiredService<IExternalScriptingShell>();

            switch (shell)
            {
                case ScriptingShell.WinPS:
                    await session.Init(shell, "powershell.exe", "\r\n", senderConnectionId);
                    break;
                case ScriptingShell.Bash:
                    await session.Init(shell, "bash", "\n", senderConnectionId);
                    break;
                case ScriptingShell.CMD:
                    await session.Init(shell, "cmd.exe", "\r\n", senderConnectionId);
                    break;
                default:
                    throw new ArgumentException($"Unknown external scripting shell type: {shell}");
            }
            _sessions.AddOrUpdate($"{shell}-{senderConnectionId}", session, (id, b) => session);
            return session;
        }
    }

    public async Task Init(ScriptingShell shell, string shellProcessName, string lineEnding, string connectionId)
    {
        _shell = shell;
        _lineEnding = lineEnding;
        _senderConnectionId = connectionId;

        var psi = new ProcessStartInfo(shellProcessName)
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            Verb = "RunAs",
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };

        var configInfo = _configService.GetConnectionInfo();
        psi.Environment.Add("DeviceId", configInfo.DeviceID);
        psi.Environment.Add("ServerUrl", configInfo.Host);

        ShellProcess = new Process
        {
            StartInfo = psi
        };
        ShellProcess.ErrorDataReceived += ShellProcess_ErrorDataReceived;
        ShellProcess.OutputDataReceived += ShellProcess_OutputDataReceived;

        ShellProcess.Start();

        ShellProcess.BeginErrorReadLine();
        ShellProcess.BeginOutputReadLine();

        _processIdleTimeout = new System.Timers.Timer(TimeSpan.FromMinutes(10))
        {
            AutoReset = false
        };
        _processIdleTimeout.Elapsed += ProcessIdleTimeout_Elapsed;
        _processIdleTimeout.Start();

        if (shell == ScriptingShell.WinPS)
        {
            await WriteInput("$VerbosePreference = \"Continue\";", TimeSpan.FromSeconds(5));
            await WriteInput("$DebugPreference = \"Continue\";", TimeSpan.FromSeconds(5));
            await WriteInput("$InformationPreference = \"Continue\";", TimeSpan.FromSeconds(5));
            await WriteInput("$WarningPreference = \"Continue\";", TimeSpan.FromSeconds(5));
        }
    }

    public async Task<ScriptResult> WriteInput(string input, TimeSpan timeout)
    {
        await _writeLock.WaitAsync();
        var sw = Stopwatch.StartNew();

        try
        {
            if (ShellProcess?.HasExited != false)
            {
                throw new InvalidOperationException("Shell process is not running.");
            }

            _processIdleTimeout.Stop();
            _processIdleTimeout.Start();
            _outputDone.Reset();

            _standardOut = "";
            _errorOut = "";
            _lastInputID = Guid.NewGuid().ToString();
            
            ShellProcess.StandardInput.Write(input + _lineEnding);
            ShellProcess.StandardInput.Write("echo " + _lastInputID + _lineEnding);

            var result = await Task.WhenAny(
                Task.Run(() =>
                {
                    return ShellProcess.WaitForExit((int)timeout.TotalMilliseconds);
                }),
                Task.Run(() =>
                {
                    return _outputDone.WaitOne();

                })).ConfigureAwait(false).GetAwaiter().GetResult();

            if (!result)
            {
                return GeneratePartialResult(input, sw.Elapsed);
            }

            return GenerateCompletedResult(input, sw.Elapsed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while writing input to scripting shell.");
            _errorOut += Environment.NewLine + ex.Message;

            // Something's wrong.  Let the next command start a new session.
            RemoveSession();
        }
        finally
        {
            _writeLock.Release();
        }

        return GeneratePartialResult(input, sw.Elapsed);
    }

    private ScriptResult GenerateCompletedResult(string input, TimeSpan runtime)
    {
        return new ScriptResult()
        {
            Shell = _shell,
            RunTime = runtime,
            ScriptInput = input,
            SenderConnectionID = _senderConnectionId,
            DeviceID = _configService.GetConnectionInfo().DeviceID,
            StandardOutput = _standardOut.Split(Environment.NewLine),
            ErrorOutput = _errorOut.Split(Environment.NewLine),
            HadErrors = !string.IsNullOrWhiteSpace(_errorOut) ||
                (ShellProcess?.HasExited == true && ShellProcess.ExitCode != 0)
        };
    }

    private ScriptResult GeneratePartialResult(string input, TimeSpan runtime)
    {
        var partialResult = new ScriptResult()
        {
            Shell = _shell,
            RunTime = runtime,
            ScriptInput = input,
            SenderConnectionID = _senderConnectionId,
            DeviceID = _configService.GetConnectionInfo().DeviceID,
            StandardOutput = _standardOut.Split(Environment.NewLine),
            ErrorOutput = (new[] { "WARNING: The command execution timed out and was forced to return before finishing.  " +
                "The results may be partial, and the terminal process has been reset.  " +
                "Please note that interactive commands aren't supported."})
                .Concat(_errorOut.Split(Environment.NewLine))
                .ToArray(),
            HadErrors = !string.IsNullOrWhiteSpace(_errorOut) ||
                (ShellProcess?.HasExited == true && ShellProcess.ExitCode != 0)
        };
        RemoveSession();
        return partialResult;
    }
    private void ProcessIdleTimeout_Elapsed(object? sender, ElapsedEventArgs e)
    {
        RemoveSession();
    }

    private void RemoveSession()
    {
        ShellProcess?.Kill();
        if (_senderConnectionId is null)
        {
            return;
        }
        _sessions.TryRemove(_senderConnectionId, out _);
    }

    private void ShellProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e?.Data != null)
        {
            _errorOut += e.Data + Environment.NewLine;
        }
    }

    private void ShellProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data?.Contains(_lastInputID) == true)
        {
            _outputDone.Set();
        }
        else
        {
            _standardOut += e.Data + Environment.NewLine;
        }

    }
}
