using Remotely_Library.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

namespace Remotely_Agent.Services
{
    partial class WindowsService : ServiceBase
    {
        public WindowsService()
        {
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
                DeviceSocket.Connect();
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
    }
}
