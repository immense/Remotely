using Microsoft.Extensions.DependencyInjection;
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
        public WindowsPS(ConfigService configService)
        {
            ConfigService = configService;
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

            ProcessIdleTimeout = new System.Timers.Timer(600_000); // 10 minutes.
            ProcessIdleTimeout.AutoReset = false;
            ProcessIdleTimeout.Elapsed += ProcessIdleTimeout_Elapsed;
            ProcessIdleTimeout.Start();
        }

        private static ConcurrentDictionary<string, WindowsPS> Sessions { get; set; } = new ConcurrentDictionary<string, WindowsPS>();
        private ConfigService ConfigService { get; }
        private string ConnectionID { get; set; }
        private string ErrorOut { get; set; }
        private string LastInputID { get; set; }
        private bool OutputDone { get; set; }
        private System.Timers.Timer ProcessIdleTimeout { get; set; }
        private Process PSProc { get; }
        private string StandardOut { get; set; }
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
                var winPS = Program.Services.GetRequiredService<WindowsPS>();

                winPS.ConnectionID = connectionID;
                Sessions.AddOrUpdate(connectionID, winPS, (id, w) => winPS);
                return winPS;
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
                CommandType = "WinPS",
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
                CommandType = "WinPS",
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
            PSProc?.Kill();
            Sessions.TryRemove(ConnectionID, out _);
        }
    }
}
