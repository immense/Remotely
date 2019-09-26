using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace Remotely.Agent.Services
{
    public class Bash
    {
        private static ConcurrentDictionary<string, Bash> Sessions { get; set; } = new ConcurrentDictionary<string, Bash>();
        private Process BashProc { get; }
        private System.Timers.Timer ProcessIdleTimeout { get; set; }
        private string ConnectionID { get; set; }
        private string LastInputID { get; set; }
        private bool OutputDone { get; set; }
        private string StandardOut { get; set; }
        private string ErrorOut { get; set; }

        public static Bash GetCurrent(string connectionID)
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
                var bash = new Bash();
                bash.ConnectionID = connectionID;
                bash.ProcessIdleTimeout = new System.Timers.Timer(600000); // 10 minutes.
                bash.ProcessIdleTimeout.AutoReset = false;
                bash.ProcessIdleTimeout.Elapsed += bash.ProcessIdleTimeout_Elapsed;
                Sessions.AddOrUpdate(connectionID, bash, (id, b) => bash);
                bash.ProcessIdleTimeout.Start();
                return bash;
            }
        }

        private void ProcessIdleTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Sessions.Remove(ConnectionID, out var outResult);
            outResult.BashProc.Kill();
        }



        private Bash()
        {
            var psi = new ProcessStartInfo("bash");
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.Verb = "RunAs";
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;

            BashProc = new Process();
            BashProc.StartInfo = psi;
            BashProc.ErrorDataReceived += CMDProc_ErrorDataReceived;
            BashProc.OutputDataReceived += CMDProc_OutputDataReceived;

            BashProc.Start();

            BashProc.BeginErrorReadLine();
            BashProc.BeginOutputReadLine();
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

        private void CMDProc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data != null)
            {
                ErrorOut += e.Data + Environment.NewLine;
            }
        }

        public GenericCommandResult WriteInput(string input, string commandID)
        {
            StandardOut = "";
            ErrorOut = "";
            lock (BashProc)
            {
                LastInputID = commandID;
                OutputDone = false;
                BashProc.StandardInput.WriteLine(input);
                BashProc.StandardInput.WriteLine("echo " + commandID);
                var startWait = DateTime.Now;
                while (!OutputDone)
                {
                    if (DateTime.Now - startWait > TimeSpan.FromSeconds(30))
                    {
                        return GeneratePartialResult();
                    }
                    Thread.Sleep(1);
                }
            }
            return GenerateCompletedResult();
        }

        private GenericCommandResult GeneratePartialResult()
        {
            OutputDone = true;
            var partialResult =  new GenericCommandResult()
            {
                CommandContextID = LastInputID,
                DeviceID = Utilities.GetConnectionInfo().DeviceID,
                CommandType = "Bash",
                StandardOutput = StandardOut,
                ErrorOutput = "WARNING: The command execution froze and was forced to return before finishing.  " +
                    "The results may be partial, and the console process has been reset.  "  +
                    "Please note that interactive commands aren't supported." + Environment.NewLine + ErrorOut
            };
            ProcessIdleTimeout_Elapsed(this, null);
            return partialResult;
        }

        private GenericCommandResult GenerateCompletedResult()
        {
            return new GenericCommandResult()
            {
                CommandContextID = LastInputID,
                DeviceID = Utilities.GetConnectionInfo().DeviceID,
                CommandType = "Bash",
                StandardOutput = StandardOut,
                ErrorOutput = ErrorOut
            };
        }

    }
}
