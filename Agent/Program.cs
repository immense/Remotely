using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Agent.Interfaces;
using Remotely.Agent.Services;
using Remotely.Shared.Enums;
using Remotely.Shared.Utilities;
using Remotely.Shared.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using Remotely.Agent.Services.Linux;
using Remotely.Agent.Services.MacOS;
using Remotely.Agent.Services.Windows;

namespace Remotely.Agent
{
    public class Program
    {

        public static IServiceProvider Services { get; set; }

        public static async Task Main(string[] args)
        {
            try
            {
                // TODO: Convert to generic host.
                BuildServices();

                await Init();

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
            serviceCollection.AddHttpClient();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConsole().AddDebug();
                var version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.0.0";
                builder.AddProvider(new FileLoggerProvider("Remotely_Agent", version));
            });

            // TODO: All these should be registered as interfaces.
            serviceCollection.AddSingleton<IAgentHubConnection, AgentHubConnection>();
            serviceCollection.AddSingleton<ICpuUtilizationSampler, CpuUtilizationSampler>();
            serviceCollection.AddHostedService(services => services.GetRequiredService<ICpuUtilizationSampler>());
            serviceCollection.AddScoped<ChatClientService>();
            serviceCollection.AddTransient<PSCore>();
            serviceCollection.AddTransient<ExternalScriptingShell>();
            serviceCollection.AddScoped<ConfigService>();
            serviceCollection.AddScoped<Uninstaller>();
            serviceCollection.AddScoped<ScriptExecutor>();
            serviceCollection.AddScoped<IProcessInvoker, ProcessInvoker>();
            serviceCollection.AddScoped<IUpdateDownloader, UpdateDownloader>();

            if (OperatingSystem.IsWindows())
            {
                serviceCollection.AddScoped<IAppLauncher, AppLauncherWin>();
                serviceCollection.AddSingleton<IUpdater, UpdaterWin>();
                serviceCollection.AddSingleton<IDeviceInformationService, DeviceInfoGeneratorWin>();
            }
            else if (OperatingSystem.IsLinux())
            {
                serviceCollection.AddScoped<IAppLauncher, AppLauncherLinux>();
                serviceCollection.AddSingleton<IUpdater, UpdaterLinux>();
                serviceCollection.AddSingleton<IDeviceInformationService, DeviceInfoGeneratorLinux>();
            }
            else if (OperatingSystem.IsMacOS())
            {
                serviceCollection.AddScoped<IAppLauncher, AppLauncherMac>();
                serviceCollection.AddSingleton<IUpdater, UpdaterMac>();
                serviceCollection.AddSingleton<IDeviceInformationService, DeviceInfoGeneratorMac>();
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


                if (OperatingSystem.IsWindows() &&
                    Process.GetCurrentProcess().SessionId == 0)
                {
                    _ = Task.Run(StartService);
                }

                await Services.GetRequiredService<IUpdater>().BeginChecking();

                await Services.GetRequiredService<IAgentHubConnection>().Connect();
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

        [SupportedOSPlatform("windows")]
        private static void StartService()
        {
            try
            {
                ServiceBase.Run(new WindowsService());
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Failed to start service.", EventType.Warning);
            }
        }
    }
}
