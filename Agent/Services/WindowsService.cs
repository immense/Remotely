using Remotely.Shared.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

namespace Remotely.Agent.Services
{
    partial class WindowsService : ServiceBase
    {
        public WindowsService()
        {
            CanHandleSessionChangeEvent = true;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            try
            {
                if (OSUtils.IsWindows)
                {
                    // Set Secure Attention Sequence policy to allow app to simulate Ctrl + Alt + Del.
                    var subkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true);
                    subkey.SetValue("SoftwareSASGeneration", "3", Microsoft.Win32.RegistryValueKind.DWord);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (OSUtils.IsWindows)
                {
                    // Remove Secure Attention Sequence policy to allow app to simulate Ctrl + Alt + Del.
                    var subkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true);
                    if (subkey.GetValue("SoftwareSASGeneration") != null)
                    {
                        subkey.DeleteValue("SoftwareSASGeneration");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                throw;
            }
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            Logger.Write($"Session changed.  Reason: {changeDescription.Reason}");
            if (changeDescription.Reason == SessionChangeReason.ConsoleDisconnect ||
                changeDescription.Reason == SessionChangeReason.RemoteDisconnect)
            {
                foreach (var screenCaster in Process.GetProcessesByName("Remotely_ScreenCast"))
                {
                    Logger.Write($"Session changed.  Kill process ID {screenCaster.Id}.");
                    screenCaster.Kill();
                }
            }
            base.OnSessionChange(changeDescription);
        }
    }
}
