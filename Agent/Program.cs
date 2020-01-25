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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Remotely.Agent
{
    public class Program
    {
        public static bool IsDebug { get; set; }

        public static IServiceProvider Services { get; set; }

        public static void Main(string[] args)
        {
            try
            {
                BuildServices();

                Task.Run(() => { Init(args); });

                Thread.Sleep(Timeout.Infinite);

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private static void BuildServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConsole().AddEventLog();
            });
            serviceCollection.AddSingleton<DeviceSocket>();
            serviceCollection.AddScoped<ChatClientService>();
            serviceCollection.AddTransient<Bash>();
            serviceCollection.AddTransient<CMD>();
            serviceCollection.AddTransient<PSCore>();
            serviceCollection.AddTransient<WindowsPS>();
            serviceCollection.AddScoped<ConfigService>();
            serviceCollection.AddScoped<Logger>();
            serviceCollection.AddScoped<Updater>();
            serviceCollection.AddScoped<Uninstaller>();
            serviceCollection.AddScoped<ScriptRunner>();
            serviceCollection.AddScoped<CommandExecutor>();
            serviceCollection.AddScoped<AppLauncher>();

            Services = serviceCollection.BuildServiceProvider();
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

        private static async Task HandleConnection()
        {
            while (true)
            {
                try
                {
                    if (!Services.GetRequiredService<DeviceSocket>().IsConnected)
                    {
                        var waitTime = new Random().Next(1000, 30000);
                        Logger.Write($"Websocket closed.  Reconnecting in {waitTime / 1000} seconds...");
                        await Task.Delay(waitTime);
                        await Services.GetRequiredService<DeviceSocket>().Connect();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
                Thread.Sleep(1000);
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

            if (!IsDebug && OSUtils.IsWindows)
            {
                _ = Task.Run(() =>
                {
                    ServiceBase.Run(new WindowsService());
                });
            }

            try
            {
                await Services.GetRequiredService<DeviceSocket>().Connect();
            }
            finally
            {
                await HandleConnection();
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
