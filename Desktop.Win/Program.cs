using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Win.Services;
using Remotely.Desktop.Win.Views;
using Remotely.Shared.Utilities;
using Remotely.Shared.Models;
using Remotely.Shared.Services;
using Remotely.Shared.Win32;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Forms;

namespace Remotely.Desktop.Win
{
    public class Program
    {
        public static Form BackgroundForm { get; private set; }
        private static ICasterSocket CasterSocket { get; set; }
        private static Conductor Conductor { get; set; }
        private static ICursorIconWatcher CursorIconWatcher { get; set; }
        private static IServiceProvider Services => ServiceContainer.Instance;
        public static async void CursorIconWatcher_OnChange(object sender, CursorInfo cursor)
        {
            if (Conductor?.Viewers?.Count > 0)
            {
                foreach (var viewer in Conductor.Viewers.Values)
                {
                    await viewer.SendCursorChange(cursor);
                }
            }
        }
        
        public static async Task Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                BuildServices();

                Conductor = Services.GetRequiredService<Conductor>();
                CasterSocket = Services.GetRequiredService<ICasterSocket>();
                Conductor.ProcessArgs(args);

                SystemEvents.SessionEnding += async (s, e) =>
                {
                    if (e.Reason == SessionEndReasons.SystemShutdown)
                    {
                        await CasterSocket.DisconnectAllViewers();
                    }
                };

                var deviceInitService = Services.GetRequiredService<IDeviceInitService>();

                var activationUri = Services.GetRequiredService<IClickOnceService>().GetActivationUri();
                if (Uri.TryCreate(activationUri, UriKind.Absolute, out var result))
                {
                    var host = $"{result.Scheme}://{result.Authority}";

                    if (!string.IsNullOrWhiteSpace(host))
                    {
                        Conductor.UpdateHost(host);
                        using var httpClient = new HttpClient();
                        try
                        {
                            var url = $"{host.TrimEnd('/')}/api/branding";
                            var query = HttpUtility.ParseQueryString(result.Query);
                            if (query?.AllKeys?.Contains("organizationid") == true)
                            {
                                url += $"?organizationId={query["organizationid"]}";
                                Conductor.UpdateOrganizationId(query["organizationid"]);
                            }
                            var branding = await httpClient.GetFromJsonAsync<BrandingInfo>(url).ConfigureAwait(false);
                            if (branding != null)
                            {
                                deviceInitService.SetBrandingInfo(branding);
                            }
                        }
                        catch { }
                    }
                }

                await deviceInitService.GetInitParams().ConfigureAwait(false);
                

                if (Conductor.Mode == Core.Enums.AppMode.Chat)
                {
                    StartUiThreads(false);
                    _ = Task.Run(async () =>
                    {
                        var chatService = Services.GetRequiredService<IChatClientService>();
                        await chatService.StartChat(Conductor.RequesterID, Conductor.OrganizationName);
                    });
                }
                else if (Conductor.Mode == Core.Enums.AppMode.Unattended)
                {
                    StartUiThreads(false);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        App.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    });
                    _ = Task.Run(StartScreenCasting);
                }
                else
                {
                    StartUiThreads(true);
                }

                WaitForAppExit();
                
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
                builder.AddConsole().AddDebug().AddEventLog();
            });

            serviceCollection.AddSingleton<ICursorIconWatcher, CursorIconWatcherWin>();
            serviceCollection.AddSingleton<IScreenCaster, ScreenCaster>();
            serviceCollection.AddSingleton<IKeyboardMouseInput, KeyboardMouseInputWin>();
            serviceCollection.AddSingleton<IClipboardService, ClipboardServiceWin>();
            serviceCollection.AddSingleton<IAudioCapturer, AudioCapturerWin>();
            serviceCollection.AddSingleton<ICasterSocket, CasterSocket>();
            serviceCollection.AddSingleton<IdleTimer>();
            serviceCollection.AddSingleton<Conductor>();
            serviceCollection.AddSingleton<IChatClientService, ChatHostService>();
            serviceCollection.AddSingleton<IChatUiService, ChatUiServiceWin>();
            serviceCollection.AddTransient<IScreenCapturer, ScreenCapturerWin>();
            serviceCollection.AddTransient<Viewer>();
            serviceCollection.AddScoped<IWebRtcSessionFactory, WebRtcSessionFactory>();
            serviceCollection.AddScoped<IFileTransferService, FileTransferServiceWin>();
            serviceCollection.AddSingleton<ISessionIndicator, SessionIndicatorWin>();
            serviceCollection.AddSingleton<IShutdownService, ShutdownServiceWin>();
            serviceCollection.AddScoped<IDtoMessageHandler, DtoMessageHandler>();
            serviceCollection.AddScoped<IRemoteControlAccessService, RemoteControlAccessServiceWin>();
            serviceCollection.AddScoped<IConfigService, ConfigServiceWin>();
            serviceCollection.AddScoped<IDeviceInitService, DeviceInitService>();
            serviceCollection.AddScoped<IClickOnceService, ClickOnceService>();

            BackgroundForm = new Form()
            {
                Visible = false,
                Opacity = 0,
                ShowIcon = false,
                ShowInTaskbar = false,
                WindowState = FormWindowState.Minimized
            };
            serviceCollection.AddSingleton((serviceProvider) => BackgroundForm);

            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Write((Exception)e.ExceptionObject);
        }

        private static async Task SendReadyNotificationToViewers()
        {

            if (Conductor.ArgDict.ContainsKey("relaunch"))
            {
                Logger.Write($"Resuming after relaunch.");
                var viewersString = Conductor.ArgDict["viewers"];
                var viewerIDs = viewersString.Split(",".ToCharArray());
                await CasterSocket.NotifyViewersRelaunchedScreenCasterReady(viewerIDs);
            }
            else
            {
                await CasterSocket.NotifyRequesterUnattendedReady(Conductor.RequesterID);
            }
        }

        private static async Task StartScreenCasting()
        {
            
            CursorIconWatcher = Services.GetRequiredService<ICursorIconWatcher>();

            await CasterSocket.Connect(Conductor.Host);
            await CasterSocket.SendDeviceInfo(Conductor.ServiceID, Environment.MachineName, Conductor.DeviceID);

            if (Win32Interop.GetCurrentDesktop(out var currentDesktopName))
            {
                Logger.Write($"Setting initial desktop to {currentDesktopName}.");
            }
            else
            {
                Logger.Write("Failed to get initial desktop name.");
            }

            if (!Win32Interop.SwitchToInputDesktop())
            {
                Logger.Write("Failed to set initial desktop.");
            }

            await SendReadyNotificationToViewers();
            Services.GetRequiredService<IdleTimer>().Start();
            CursorIconWatcher.OnChange += CursorIconWatcher_OnChange;
            Services.GetRequiredService<IClipboardService>().BeginWatching();
            Services.GetRequiredService<IKeyboardMouseInput>().Init();
        }

        private static void StartUiThreads(bool createMainWindow)
        {
            var wpfUiThread = new Thread(() =>
            {
                var app = new App();
                app.InitializeComponent();

                if (createMainWindow)
                {
                    app.Run(new MainWindow());
                }
                else
                {
                    app.Run();
                }
            });
            wpfUiThread.TrySetApartmentState(ApartmentState.STA);
            wpfUiThread.IsBackground = true;
            wpfUiThread.Start();

            var winformsThread = new Thread(() =>
            {
                System.Windows.Forms.Application.Run(BackgroundForm);
            })
            {
                IsBackground = true
            };
            winformsThread.TrySetApartmentState(ApartmentState.STA);
            winformsThread.Start();

            // Wait until WPF app has initialized before moving on.
            while (App.Current is null)
            {
                Thread.Sleep(100);
            }
            Logger.Write("Background UI apps started.");
        }

        private static void WaitForAppExit()
        {
            var appExitEvent = new ManualResetEventSlim();
            App.Current.Dispatcher.Invoke(() =>
            {
                App.Current.Exit += (s, a) =>
                {
                    appExitEvent.Set();
                };
            });
            appExitEvent.Wait();
        }
    }
}
