using Microsoft.Extensions.DependencyInjection;
using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Timers;

namespace Remotely.Agent.Services
{
    public class PSCore
    {
        private readonly ConfigService _configService;
        private readonly ConnectionInfo _connectionInfo;
        private CommandCompletion _lastCompletion;
        private string _lastInputText;
        private readonly PowerShell _powershell;

        public PSCore(ConfigService configService)
        {
            _configService = configService;
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

        private static ConcurrentDictionary<string, PSCore> Sessions { get; set; } = new ConcurrentDictionary<string, PSCore>();

        public static PSCore GetCurrent(string senderConnectionId)
        {
            if (Sessions.TryGetValue(senderConnectionId, out var session))
            {
                return session;
            }
            else
            {
                session = Program.Services.GetRequiredService<PSCore>();
                session.SenderConnectionId = senderConnectionId;
                Sessions.AddOrUpdate(senderConnectionId, session, (id, b) => session);
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
            var sw = Stopwatch.StartNew();

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
    }
}
