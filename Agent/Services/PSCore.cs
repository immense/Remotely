using Remotely.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;

namespace Remotely.Agent.Services
{
    public class PSCore
    {
        public PSCore(ConfigService configService)
        {
            ConfigService = configService;
            PS = PowerShell.Create();
            PS.AddScript(@"$VerbosePreference = ""Continue"";
                            $DebugPreference = ""Continue"";
                            $InformationPreference = ""Continue"";
                            $WarningPreference = ""Continue"";");
            PS.Invoke();

            ProcessIdleTimeout = new Timer(600_000); // 10 minutes.
            ProcessIdleTimeout.AutoReset = false;
            ProcessIdleTimeout.Elapsed += ProcessIdleTimeout_Elapsed;
            ProcessIdleTimeout.Start();
        }

        public string ConnectionID { get; private set; }

        private static ConcurrentDictionary<string, PSCore> Sessions { get; set; } = new ConcurrentDictionary<string, PSCore>();

        private ConfigService ConfigService { get; }

        private Timer ProcessIdleTimeout { get; set; }

        private PowerShell PS { get; set; }

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
                var psCore = Program.Services.GetRequiredService<PSCore>();
                psCore.ConnectionID = connectionID;
                Sessions.AddOrUpdate(connectionID, psCore, (id, p) => psCore);
                return psCore;
            }
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
                DeviceID = ConfigService.GetConnectionInfo().DeviceID,
                DebugOutput = debugOut,
                ErrorOutput = errorOut,
                VerboseOutput = verboseOut,
                HostOutput = hostOutput,
                InformationOutput = infoOut,
                WarningOutput = warningOut
            };
        }

        private void ProcessIdleTimeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            Sessions.TryRemove(ConnectionID, out _);
            PS?.Dispose();
        }
    }
}
