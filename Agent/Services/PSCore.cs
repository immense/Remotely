using Remotely.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Timers;

namespace Remotely.Agent.Services
{
    public class PSCore
    {
        private static ConcurrentDictionary<string, PSCore> Sessions { get; set; } = new ConcurrentDictionary<string, PSCore>();
        private Timer ProcessIdleTimeout { get; set; }
        public static PSCore GetCurrent(string connectionID)
        {
            if (Sessions.ContainsKey(connectionID))
            {
                var psCore = Sessions[connectionID];
                psCore.ProcessIdleTimeout.Stop();
                psCore.ProcessIdleTimeout.Start();
                return psCore;
            }
            else
            {
                var psCore = new PSCore();
                psCore.ProcessIdleTimeout = new Timer(600000); // 10 minutes.
                psCore.ProcessIdleTimeout.AutoReset = false;
                psCore.ProcessIdleTimeout.Elapsed += (sender, args) =>
                {
                    Sessions.Remove(connectionID, out var pSCore);
                };
                Sessions.AddOrUpdate(connectionID, psCore, (id, p) => psCore);
                psCore.ProcessIdleTimeout.Start();
                return psCore;
            }
        }
        private PowerShell PS { get; set; }

        private PSCore()
        {
            PS = PowerShell.Create();
            PS.AddScript(@"$VerbosePreference = ""Continue"";
                            $DebugPreference = ""Continue"";
                            $InformationPreference = ""Continue"";
                            $WarningPreference = ""Continue"";");
            PS.Invoke();
        }

        public PSCoreCommandResult WriteInput(string input, string commandID)
        {
            PS.Commands.Clear();
            PS.AddScript(input);
            var results = PS.Invoke();
           
            var ps = PowerShell.Create();
            ps.AddScript("$args[0] | Out-String");
            ps.AddArgument(results);
            var hostOutput = (ps.Invoke()[0].BaseObject as string);

            var verboseOut = PS.Streams.Verbose.ReadAll().Select(x => x.Message).ToList();
            var debugOut = PS.Streams.Debug.ReadAll().Select(x => x.Message).ToList();
            var errorOut = PS.Streams.Error.ReadAll().Select(x => x.Exception.ToString() + Environment.NewLine + x.ScriptStackTrace).ToList();
            var infoOut = PS.Streams.Information.Select(x => x.MessageData.ToString()).ToList();
            var warningOut = PS.Streams.Warning.Select(x => x.Message).ToList();

            PS.Streams.ClearStreams();
            PS.Commands.Clear();
            
            return new PSCoreCommandResult()
            {
                CommandContextID = commandID,
                DeviceID = Utilities.GetConnectionInfo().DeviceID,
                DebugOutput = debugOut,
                ErrorOutput = errorOut,
                VerboseOutput = verboseOut,
                HostOutput = hostOutput,
                InformationOutput = infoOut,
                WarningOutput = warningOut
            };
        }

    }
}
