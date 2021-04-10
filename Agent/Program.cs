using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Agent.Interfaces;
using Remotely.Agent.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Agent
{
    public class Program
    {

        public static IServiceProvider Services { get; set; }

        public static async Task Main(string[] args)
        {
            try
            {
                BuildServices();

                await Init();

                _ = Task.Run(Services.GetRequiredService<AgentSocket>().HandleConnection);

                await Task.Delay(-1);

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                throw;
            }
        }

        private static void BuildServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConsole().AddDebug();
            });
            serviceCollection.AddSingleton<AgentSocket>();
            serviceCollection.AddScoped<ChatClientService>();
            serviceCollection.AddTransient<PSCore>();
            serviceCollection.AddTransient<ExternalScriptingShell>();
            serviceCollection.AddScoped<ConfigService>();
            serviceCollection.AddScoped<Uninstaller>();
            serviceCollection.AddScoped<ScriptExecutor>();

            if (EnvironmentHelper.IsWindows)
            {
                serviceCollection.AddScoped<IAppLauncher, AppLauncherWin>();
                serviceCollection.AddSingleton<IUpdater, UpdaterWin>();
                serviceCollection.AddSingleton<IDeviceInformationService, DeviceInformationServiceWin>();
            }
            else if (EnvironmentHelper.IsLinux)
            {
                serviceCollection.AddScoped<IAppLauncher, AppLauncherLinux>();
                serviceCollection.AddSingleton<IUpdater, UpdaterLinux>();
                serviceCollection.AddSingleton<IDeviceInformationService, DeviceInformationServiceLinux>();
            }
            else if (EnvironmentHelper.IsMac)
            {
                // TODO: Mac.
            }
            else
            {
                throw new NotSupportedException("Operating system not supported.");
            }

            Services = serviceCollection.BuildServiceProvider();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write(e.ExceptionObject as Exception);
        }

        private static async Task Init()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                SetWorkingDirectory();


                if (EnvironmentHelper.IsWindows &&
                    Process.GetCurrentProcess().SessionId == 0)
                {
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            ServiceBase.Run(new WindowsService());
                        }
                        catch (Exception ex)
                        {
                            Logger.Write(ex, "Failed to start service.", EventType.Warning);
                        }
                    });
                }

                await Services.GetRequiredService<IUpdater>().BeginChecking();

                await Services.GetRequiredService<AgentSocket>().Connect();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private static void SetWorkingDirectory()
        {
            var assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var assemblyDir = Path.GetDirectoryName(assemblyPath);
            Directory.SetCurrentDirectory(assemblyDir);
        }
    }
}
