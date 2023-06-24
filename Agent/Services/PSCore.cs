using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Timers;
using static Immense.RemoteControl.Desktop.Native.Windows.User32;

namespace Remotely.Agent.Services
{
    public interface IPSCore
    {
        string SenderConnectionId { get; }

        CommandCompletion GetCompletions(string inputText, int currentIndex, bool? forward);
        ScriptResult WriteInput(string input);
    }

    public class PSCore : IPSCore
    {
        private static readonly ConcurrentDictionary<string, PSCore> _sessions = new ConcurrentDictionary<string, PSCore>();
        private readonly IConfigService _configService;
        private readonly ConnectionInfo _connectionInfo;
        private readonly ILogger<PSCore> _logger;
        private readonly PowerShell _powershell;
        private CommandCompletion _lastCompletion;
        private string _lastInputText;
        public PSCore(
            IConfigService configService,
            ILogger<PSCore> logger)
        {
            _configService = configService;
            _logger = logger;
            _connectionInfo = _configService.GetConnectionInfo();

            _powershell = PowerShell.Create();

            _powershell.AddScript($@"$VerbosePreference = ""Continue"";
                            $DebugPreference = ""Continue"";
                            $InformationPreference = ""Continue"";
                            $WarningPreference = ""Continue"";
                            $env:DeviceId = ""{_connectionInfo.DeviceID}"";
                            $env:ServerUrl = ""{_connectionInfo.Host}""");

            _powershell.Invoke();
        }

        public string SenderConnectionId { get; private set; }
        // TODO: Turn into cache and factory.
        public static PSCore GetCurrent(string senderConnectionId)
        {
            if (_sessions.TryGetValue(senderConnectionId, out var session))
            {
                return session;
            }
            else
            {
                session = Program.Services.GetRequiredService<PSCore>();
                session.SenderConnectionId = senderConnectionId;
                _sessions.AddOrUpdate(senderConnectionId, session, (id, b) => session);
                return session;
            }
        }

        public CommandCompletion GetCompletions(string inputText, int currentIndex, bool? forward)
        {
            if (_lastCompletion is null ||
                inputText != _lastInputText)
            {
                _lastInputText = inputText;
                _lastCompletion = CommandCompletion.CompleteInput(inputText, currentIndex, new(), _powershell);
            }

            if (forward.HasValue)
            {
                _lastCompletion.GetNextResult(forward.Value);
            }

            return _lastCompletion;
        }

        public ScriptResult WriteInput(string input)
        {
            var deviceId = _configService.GetConnectionInfo().DeviceID;
            var sw = Stopwatch.StartNew();

            try
            {

                _powershell.Streams.ClearStreams();
                _powershell.Commands.Clear();

                _powershell.AddScript(input);
                var results = _powershell.Invoke();

                using var ps = PowerShell.Create();
                ps.AddScript("$args[0] | Out-String");
                ps.AddArgument(results);
                var hostOutput = (string)ps.Invoke()[0].BaseObject;

                var verboseOut = _powershell.Streams.Verbose.ReadAll().Select(x => x.Message);
                var debugOut = _powershell.Streams.Debug.ReadAll().Select(x => x.Message);
                var errorOut = _powershell.Streams.Error.ReadAll().Select(x => x.Exception.ToString() + Environment.NewLine + x.ScriptStackTrace);
                var infoOut = _powershell.Streams.Information.Select(x => x.MessageData.ToString());
                var warningOut = _powershell.Streams.Warning.Select(x => x.Message);

                var standardOut = hostOutput.Split(Environment.NewLine)
                    .Concat(infoOut)
                    .Concat(debugOut)
                    .Concat(verboseOut);

                var errorAndWarningOut = errorOut.Concat(warningOut).ToArray();


                return new ScriptResult()
                {
                    DeviceID = _configService.GetConnectionInfo().DeviceID,
                    SenderConnectionID = SenderConnectionId,
                    ScriptInput = input,
                    Shell = Shared.Enums.ScriptingShell.PSCore,
                    StandardOutput = standardOut.ToArray(),
                    ErrorOutput = errorAndWarningOut,
                    RunTime = sw.Elapsed,
                    HadErrors = _powershell.HadErrors || errorAndWarningOut.Any()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while writing input to PSCore.");
                return new ScriptResult()
                {
                    DeviceID = deviceId,
                    SenderConnectionID = SenderConnectionId,
                    ScriptInput = input,
                    Shell = Shared.Enums.ScriptingShell.PSCore,
                    StandardOutput = Array.Empty<string>(),
                    ErrorOutput = new[] { "Error while writing input." },
                    RunTime = sw.Elapsed,
                    HadErrors = true
                };
            }
        }
    }
}
