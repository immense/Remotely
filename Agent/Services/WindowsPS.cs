using Microsoft.Extensions.DependencyInjection;
using Remotely.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace Remotely.Agent.Services
{
    public class WindowsPS
    {
        public WindowsPS(ConfigService configService)
        {
            ConfigService = configService;
            var psi = new ProcessStartInfo("powershell.exe")
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "RunAs",
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            PSProc = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };
            PSProc.ErrorDataReceived += CMDProc_ErrorDataReceived;
            PSProc.OutputDataReceived += CMDProc_OutputDataReceived;

            PSProc.Start();

            PSProc.BeginErrorReadLine();
            PSProc.BeginOutputReadLine();

            ProcessIdleTimeout = new System.Timers.Timer(TimeSpan.FromMinutes(10).TotalMilliseconds)
            {
                AutoReset = false
            };
            ProcessIdleTimeout.Elapsed += ProcessIdleTimeout_Elapsed;
            ProcessIdleTimeout.Start();
        }

        private static ConcurrentDictionary<string, WindowsPS> Sessions { get; set; } = new ConcurrentDictionary<string, WindowsPS>();
        private ConfigService ConfigService { get; }
        private string ConnectionID { get; set; }
        private string ErrorOut { get; set; }
        private string LastInputID { get; set; }
        private ManualResetEvent OutputDone { get; } = new ManualResetEvent(false);
        private System.Timers.Timer ProcessIdleTimeout { get; set; }
        private Process PSProc { get; }
        private string StandardOut { get; set; }
        public static WindowsPS GetCurrent(string connectionID)
        {
            if (Sessions.TryGetValue(connectionID, out var session))
            {
                session.ProcessIdleTimeout.Stop();
                session.ProcessIdleTimeout.Start();
                return session;
            }
            else
            {
                session = Program.Services.GetRequiredService<WindowsPS>();
                session.ConnectionID = connectionID;
                Sessions.AddOrUpdate(connectionID, session, (id, b) => session);
                return session;
            }
        }

        public GenericCommandResult WriteInput(string input, string commandID)
        {
            StandardOut = "";
            ErrorOut = "";
            lock (PSProc)
            {
                LastInputID = commandID;
                OutputDone.Reset();
                PSProc.StandardInput.WriteLine(input);
                PSProc.StandardInput.WriteLine("echo " + commandID);
                if (!OutputDone.WaitOne(TimeSpan.FromSeconds(30)))
                {
                    return GeneratePartialResult();
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
                CommandType = "WinPS",
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
                CommandType = "WinPS",
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
            PSProc?.Kill();
            Sessions.TryRemove(ConnectionID, out _);
        }
    }
}
