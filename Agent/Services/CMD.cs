using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
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
        private ManualResetEvent OutputDone { get; } = new ManualResetEvent(false);
        private System.Timers.Timer ProcessIdleTimeout { get; set; }
        private string StandardOut { get; set; }
        public static CMD GetCurrent(string connectionID)
        {
            if (Sessions.TryGetValue(connectionID, out var session))
            {
                session.ProcessIdleTimeout.Stop();
                session.ProcessIdleTimeout.Start();
                return session;
            }
            else
            {
                session = Program.Services.GetRequiredService<CMD>();
                session.ConnectionID = connectionID;
                Sessions.AddOrUpdate(connectionID, session, (id, b) => session);
                return session;
            }
        }

        public GenericCommandResult WriteInput(string input, string commandID)
        {
            StandardOut = "";
            ErrorOut = "";
            lock (CMDProc)
            {
                LastInputID = commandID;
                OutputDone.Reset();
                CMDProc.StandardInput.WriteLine(input);
                CMDProc.StandardInput.WriteLine("echo " + commandID);
                if (!OutputDone.WaitOne(TimeSpan.FromSeconds(30)))
                {
                    return GeneratePartialResult();
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
                OutputDone.Set();
            }
            else
            {
                StandardOut += e.Data + Environment.NewLine;
            }

        }

        private GenericCommandResult GenerateCompletedResult()
        {
            return new GenericCommandResult()
            {
                CommandResultID = LastInputID,
                DeviceID = ConfigService.GetConnectionInfo().DeviceID,
                CommandType = "CMD",
                StandardOutput = StandardOut,
                ErrorOutput = ErrorOut
            };
        }

        private GenericCommandResult GeneratePartialResult()
        {
            var partialResult = new GenericCommandResult()
            {
                CommandResultID = LastInputID,
                DeviceID = ConfigService.GetConnectionInfo().DeviceID,
                CommandType = "CMD",
                StandardOutput = StandardOut,
                ErrorOutput = "WARNING: The command execution timed out and was forced to return before finishing.  " +
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
