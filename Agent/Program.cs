using Remotely.Agent.Services;
using Remotely.Shared.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Remotely.Agent
{
    public class Program
    {
        public static bool IsDebug { get; set; }

        public static void Main(string[] args)
        {
            try
            {

                Task.Run(()=> { Init(args); });

                Thread.Sleep(Timeout.Infinite);

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private static async void Init(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

#if DEBUG
            IsDebug = true;
#endif

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

            if (!IsDebug && OSUtils.IsWindows)
            {
                _ = Task.Run(() =>
                {
                    ServiceBase.Run(new WindowsService());
                });
            }

            try
            {
                await DeviceSocket.Connect();
            }
            finally
            {
                await HandleConnection();
            }
        }

        private static async Task HandleConnection()
        {
            while (true)
            {
                try
                {
                    if (!DeviceSocket.IsConnected)
                    {
                        var waitTime = new Random().Next(1000, 30000);
                        Logger.Write($"Websocket closed.  Reconnecting in {waitTime / 1000} seconds...");
                        await Task.Delay(waitTime);
                        await DeviceSocket.Connect();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
                Thread.Sleep(1000);
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
