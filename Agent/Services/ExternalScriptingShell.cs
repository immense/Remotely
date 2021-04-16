using Microsoft.Extensions.DependencyInjection;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Agent.Services
{
    public interface IExternalScriptingShell
    {
       
    }

    public class ExternalScriptingShell : IExternalScriptingShell
    {
        private static readonly ConcurrentDictionary<string, ExternalScriptingShell> _sessions = new();
        private readonly ConfigService _configService;
        private string _lineEnding;
        private ScriptingShell _shell;

        public ExternalScriptingShell(ConfigService configService)
        {
            _configService = configService;
        }

        private string ErrorOut { get; set; }

        private string LastInputID { get; set; }

        private ManualResetEvent OutputDone { get; } = new(false);

        private System.Timers.Timer ProcessIdleTimeout { get; set; }

        private string SenderConnectionId { get; set; }

        private Process ShellProcess { get; set; }

        private string StandardOut { get; set; }

        private Stopwatch Stopwatch { get; set; }

        public static ExternalScriptingShell GetCurrent(ScriptingShell shell, string senderConnectionId)
        {
            if (_sessions.TryGetValue($"{shell}-{senderConnectionId}", out var session))
            {
                session.ProcessIdleTimeout.Stop();
                session.ProcessIdleTimeout.Start();
                return session;
            }
            else
            {
                session = Program.Services.GetRequiredService<ExternalScriptingShell>();

                switch (shell)
                {
                    case ScriptingShell.WinPS:
                        session.Init(shell, "powershell.exe", "\r\n", senderConnectionId);
                        break;
                    case ScriptingShell.Bash:
                        session.Init(shell, "bash", "\n", senderConnectionId);
                        break;
                    case ScriptingShell.CMD:
                        session.Init(shell, "cmd.exe", "\r\n", senderConnectionId);
                        break;
                    default:
                        throw new ArgumentException($"Unknown external scripting shell type: {shell}");
                }
                _sessions.AddOrUpdate($"{shell}-{senderConnectionId}", session, (id, b) => session);
                return session;
            }
        }

        public ScriptResult WriteInput(string input, TimeSpan timeout)
        {
            StandardOut = "";
            ErrorOut = "";
            Stopwatch = Stopwatch.StartNew();
            lock (ShellProcess)
            {
                LastInputID = Guid.NewGuid().ToString();
                OutputDone.Reset();
                ShellProcess.StandardInput.Write(input + _lineEnding);
                ShellProcess.StandardInput.Write("echo " + LastInputID + _lineEnding);

                var result = Task.WhenAny(
                    Task.Run(() =>
                    {
                        return ShellProcess.WaitForExit((int)timeout.TotalMilliseconds);
                    }),
                    Task.Run(() =>
                    {
                        return OutputDone.WaitOne();

                    })).ConfigureAwait(false).GetAwaiter().GetResult();

                if (!result.Result)
                {
                    return GeneratePartialResult(input);
                }
            }
            return GenerateCompletedResult(input);
        }

        private ScriptResult GenerateCompletedResult(string input)
        {
            return new ScriptResult()
            {
                Shell = _shell,
                RunTime = Stopwatch.Elapsed,
                ScriptInput = input,
                SenderConnectionID = SenderConnectionId,
                DeviceID = _configService.GetConnectionInfo().DeviceID,
                StandardOutput = StandardOut.Split(Environment.NewLine),
                ErrorOutput = ErrorOut.Split(Environment.NewLine),
                HadErrors = !string.IsNullOrWhiteSpace(ErrorOut) ||
                    (ShellProcess.HasExited && ShellProcess.ExitCode != 0)
            };
        }

        private ScriptResult GeneratePartialResult(string input)
        {
            var partialResult = new ScriptResult()
            {
                Shell = _shell,
                RunTime = Stopwatch.Elapsed,
                ScriptInput = input,
                SenderConnectionID = SenderConnectionId,
                DeviceID = _configService.GetConnectionInfo().DeviceID,
                StandardOutput = StandardOut.Split(Environment.NewLine),
                ErrorOutput = (new[] { "WARNING: The command execution timed out and was forced to return before finishing.  " +
                    "The results may be partial, and the terminal process has been reset.  " +
                    "Please note that interactive commands aren't supported."})
                    .Concat(ErrorOut.Split(Environment.NewLine))
                    .ToArray(),
                HadErrors = !string.IsNullOrWhiteSpace(ErrorOut) ||
                    (ShellProcess.HasExited && ShellProcess.ExitCode != 0)
            };
            ProcessIdleTimeout_Elapsed(this, null);
            return partialResult;
        }

        private void Init(ScriptingShell shell, string shellProcessName, string lineEnding, string connectionId)
        {
            _shell = shell;
            _lineEnding = lineEnding;
            SenderConnectionId = connectionId;

            var psi = new ProcessStartInfo(shellProcessName)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "RunAs",
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            var connectionInfo = _configService.GetConnectionInfo();
            psi.Environment.Add("DeviceId", connectionInfo.DeviceID);
            psi.Environment.Add("ServerUrl", connectionInfo.Host);

            ShellProcess = new Process
            {
                StartInfo = psi
            };
            ShellProcess.ErrorDataReceived += ShellProcess_ErrorDataReceived;
            ShellProcess.OutputDataReceived += ShellProcess_OutputDataReceived;

            ShellProcess.Start();

            ShellProcess.BeginErrorReadLine();
            ShellProcess.BeginOutputReadLine();

            ProcessIdleTimeout = new System.Timers.Timer(TimeSpan.FromMinutes(10).TotalMilliseconds)
            {
                AutoReset = false
            };
            ProcessIdleTimeout.Elapsed += ProcessIdleTimeout_Elapsed;
            ProcessIdleTimeout.Start();

            if (shell == ScriptingShell.WinPS)
            {
                WriteInput("$VerbosePreference = \"Continue\";", TimeSpan.FromSeconds(5));
                WriteInput("$DebugPreference = \"Continue\";", TimeSpan.FromSeconds(5));
                WriteInput("$InformationPreference = \"Continue\";", TimeSpan.FromSeconds(5));
                WriteInput("$WarningPreference = \"Continue\";", TimeSpan.FromSeconds(5));
            }
        }
        private void ProcessIdleTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ShellProcess?.Kill();
            _sessions.TryRemove(SenderConnectionId, out _);
        }

        private void ShellProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data != null)
            {
                ErrorOut += e.Data + Environment.NewLine;
            }
        }

        private void ShellProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data?.Contains(LastInputID) == true)
            {
                OutputDone.Set();
            }
            else
            {
                StandardOut += e.Data + Environment.NewLine;
            }

        }
    }
}
