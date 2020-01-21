using Remotely.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Remotely.Agent.Services
{
    public class CMD
    {
        public CMD(ConfigService configService)
        {
            ConfigService = configService;
            var psi = new ProcessStartInfo("cmd.exe");
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.Verb = "RunAs";
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;

            CMDProc = new Process();
            CMDProc.StartInfo = psi;
            CMDProc.ErrorDataReceived += CMDProc_ErrorDataReceived;
            CMDProc.OutputDataReceived += CMDProc_OutputDataReceived;

            CMDProc.Start();

            CMDProc.BeginErrorReadLine();
            CMDProc.BeginOutputReadLine();

            ProcessIdleTimeout = new System.Timers.Timer(600_000); // 10 minutes.
            ProcessIdleTimeout.AutoReset = false;
            ProcessIdleTimeout.Elapsed += ProcessIdleTimeout_Elapsed;
            ProcessIdleTimeout.Start();
        }

        private static ConcurrentDictionary<string, CMD> Sessions { get; set; } = new ConcurrentDictionary<string, CMD>();
        private Process CMDProc { get; }
        private ConfigService ConfigService { get; }
        private string ConnectionID { get; set; }

        private string ErrorOut { get; set; }
        private string LastInputID { get; set; }
        private bool OutputDone { get; set; }
        private System.Timers.Timer ProcessIdleTimeout { get; set; }
        private string StandardOut { get; set; }
        public static CMD GetCurrent(string connectionID)
        {
            if (Sessions.ContainsKey(connectionID))
            {
                var bash = Sessions[connectionID];
                bash.ProcessIdleTimeout.Stop();
                bash.ProcessIdleTimeout.Start();
                return bash;
            }
            else
            {
                var cmd = Program.Services.GetRequiredService<CMD>();
                cmd.ConnectionID = connectionID;
                Sessions.AddOrUpdate(connectionID, cmd, (id, c) => cmd);
                return cmd;
            }
        }

        public GenericCommandResult WriteInput(string input, string commandID)
        {
            StandardOut = "";
            ErrorOut = "";
            lock (CMDProc)
            {
                LastInputID = commandID;
                OutputDone = false;
                CMDProc.StandardInput.WriteLine(input);
                CMDProc.StandardInput.WriteLine("echo " + commandID);
                var startWait = DateTime.Now;
                while (!OutputDone)
                {
                    if (DateTime.Now - startWait > TimeSpan.FromSeconds(30))
                    {
                        return GeneratePartialResult();
                    }
                    Thread.Sleep(1);
                }
                return GenerateCompletedResult();
            }

        }

        private void CMDProc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data != null)
            {
                ErrorOut += e.Data + Environment.NewLine;
            }
        }

        private void CMDProc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data?.Contains(LastInputID) == true)
            {
                OutputDone = true;
            }
            else if (!OutputDone)
            {
                StandardOut += e.Data + Environment.NewLine;
            }

        }

        private GenericCommandResult GenerateCompletedResult()
        {
            return new GenericCommandResult()
            {
                CommandContextID = LastInputID,
                DeviceID = ConfigService.GetConnectionInfo().DeviceID,
                CommandType = "CMD",
                StandardOutput = StandardOut,
                ErrorOutput = ErrorOut
            };
        }

        private GenericCommandResult GeneratePartialResult()
        {
            OutputDone = true;
            var partialResult = new GenericCommandResult()
            {
                CommandContextID = LastInputID,
                DeviceID = ConfigService.GetConnectionInfo().DeviceID,
                CommandType = "CMD",
                StandardOutput = StandardOut,
                ErrorOutput = "WARNING: The command execution froze and was forced to return before finishing.  " +
                    "The results may be partial, and the console process has been reset.  " +
                    "Please note that interactive commands aren't supported." + Environment.NewLine + ErrorOut
            };
            ProcessIdleTimeout_Elapsed(this, null);
            return partialResult;
        }

        private void ProcessIdleTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CMDProc?.Kill();
            Sessions.TryRemove(ConnectionID, out _);
        }
    }
}
