using Remotely.Agent.Services;
using System;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remotely.Shared.Utilities;

namespace Remotely.Agent
{
    public class Program
    {

        public static IServiceProvider Services { get; set; }

        public static void Main(string[] args)
        {
            try
            {
                BuildServices();

                Task.Run(() => {_ = Init(); });

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
                builder.AddConsole().AddDebug();
            });
            serviceCollection.AddSingleton<DeviceSocket>();
            serviceCollection.AddScoped<ChatClientService>();
            serviceCollection.AddTransient<Bash>();
            serviceCollection.AddTransient<CMD>();
            serviceCollection.AddTransient<PSCore>();
            serviceCollection.AddTransient<WindowsPS>();
            serviceCollection.AddScoped<ConfigService>();
            serviceCollection.AddSingleton<Updater>();
            serviceCollection.AddScoped<Uninstaller>();
            serviceCollection.AddScoped<ScriptRunner>();
            serviceCollection.AddScoped<CommandExecutor>();
            serviceCollection.AddScoped<AppLauncher>();

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


                if (!EnvironmentHelper.IsDebug && EnvironmentHelper.IsWindows)
                {
                    _ = Task.Run(() =>
                    {
                        ServiceBase.Run(new WindowsService());
                    });
                }

                await Services.GetRequiredService<Updater>().BeginChecking();

                await Services.GetRequiredService<DeviceSocket>().Connect();
            }
            finally
            {
                await Services.GetRequiredService<DeviceSocket>().HandleConnection();
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
