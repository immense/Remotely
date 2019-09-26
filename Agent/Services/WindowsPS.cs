using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Remotely.Agent.Services
{
    public class WindowsPS
    {
        private static ConcurrentDictionary<string, WindowsPS> Sessions { get; set; } = new ConcurrentDictionary<string, WindowsPS>();
        private string ConnectionID { get; set; }
        private System.Timers.Timer ProcessIdleTimeout { get; set; }
        private string LastInputID { get; set; }
        private bool OutputDone { get; set; }
        private string StandardOut { get; set; }
        private string ErrorOut { get; set; }
        public static WindowsPS GetCurrent(string connectionID)
        {
            if (Sessions.ContainsKey(connectionID))
            {
                var winPS = Sessions[connectionID];
                winPS.ProcessIdleTimeout.Stop();
                winPS.ProcessIdleTimeout.Start();
                return winPS;
            }
            else
            {
                var winPS = new WindowsPS();
                winPS.ConnectionID = connectionID;
                winPS.ProcessIdleTimeout = new System.Timers.Timer(600000); // 10 minutes.
                winPS.ProcessIdleTimeout.AutoReset = false;
                winPS.ProcessIdleTimeout.Elapsed += winPS.ProcessIdleTimeout_Elapsed;
                Sessions.AddOrUpdate(connectionID, winPS, (id, w) => winPS);
                winPS.ProcessIdleTimeout.Start();
                return winPS;
            }
        }

        private void ProcessIdleTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Sessions.Remove(ConnectionID, out var outResult);
            outResult.PSProc.Kill();
        }

        private Process PSProc { get; }

        private WindowsPS()
        {
            var psi = new ProcessStartInfo("powershell.exe");
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.Verb = "RunAs";
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;

            PSProc = new Process();
            PSProc.StartInfo = psi;
            PSProc.EnableRaisingEvents = true;
            PSProc.ErrorDataReceived += CMDProc_ErrorDataReceived;
            PSProc.OutputDataReceived += CMDProc_OutputDataReceived;

            PSProc.Start();

            PSProc.BeginErrorReadLine();
            PSProc.BeginOutputReadLine();
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
            lock (PSProc)
            {
                LastInputID = commandID;
                OutputDone = false;
                PSProc.StandardInput.WriteLine(input);
                PSProc.StandardInput.WriteLine("echo " + commandID);
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
            var partialResult = new GenericCommandResult()
            {
                CommandContextID = LastInputID,
                DeviceID = Utilities.GetConnectionInfo().DeviceID,
                CommandType = "WinPS",
                StandardOutput = StandardOut,
                ErrorOutput = "WARNING: The command execution froze and was forced to return before finishing.  " +
                    "The results may be partial, and the console process has been reset.  " +
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
                CommandType = "WinPS",
                StandardOutput = StandardOut,
                ErrorOutput = ErrorOut
            };
        }
    }
}
