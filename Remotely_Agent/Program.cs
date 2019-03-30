using Remotely_Agent.Services;
using Remotely_Library.Services;
using Remotely_Library.Win32;
using Remotely_Library.Win32_Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Remotely_Agent
{
    public class Program
    {
        public static bool IsDebug { get; set; }
        static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                SetWorkingDirectory();
                var argDict = ProcessArgs(args);

                JsonConvert.DefaultSettings = () =>
                {
                    var settings = new JsonSerializerSettings();
                    settings.Error = (sender, arg) =>
                    {
                        arg.ErrorContext.Handled = true;
                    };
                    return settings;
                };

                if (argDict.ContainsKey("update"))
                {
                    Updater.CoreUpdate();
                }

                if (OSUtils.IsWindows)
                {
#if DEBUG
                    IsDebug = true;
                    DeviceSocket.Connect();
#else
                ServiceBase.Run(new WindowsService());
#endif
                }
                else
                {
                    DeviceSocket.Connect();
                }

                while (true)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                throw;
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write(e.ExceptionObject as Exception);
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

        private static Dictionary<string,string> ProcessArgs(string[] args)
        {
            var argDict = new Dictionary<string, string>();
            
            for (var i = 0; i < args.Length; i += 2)
            {
                var key = args?[i];
                if (key != null)
                {
                    key = key.Trim().Replace("-", "").ToLower();
                    var value = args?[i + 1];
                    if (value != null)
                    {
                        argDict[key] = args[i + 1].Trim();
                    }
                }
               
            }
            return argDict;
        }

        private static void SetWorkingDirectory()
        {
            var assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var assemblyDir = Path.GetDirectoryName(assemblyPath);
            Directory.SetCurrentDirectory(assemblyDir);
        }
    }
}
