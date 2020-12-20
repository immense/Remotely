using Microsoft.Extensions.DependencyInjection;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Shared.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Remotely.Desktop.Win.Services
{
    public class SessionIndicatorWin : ISessionIndicator
    {
        private Container _container;
        private ContextMenuStrip _contextMenuStrip;
        private NotifyIcon _notifyIcon;
        public SessionIndicatorWin(Form backgroundForm)
        {
            BackgroundForm = backgroundForm;
        }

        private Form BackgroundForm { get; }

        public void Show()
        {
            try
            {
                if (_notifyIcon != null)
                {
                    return;
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    App.Current.Exit += App_Exit;
                });

                BackgroundForm.Invoke(new Action(() =>
                {
                    _container = new Container();
                    _contextMenuStrip = new ContextMenuStrip(_container);
                    _contextMenuStrip.Items.Add("Exit", null, ExitMenuItem_Click);

                    _notifyIcon = new NotifyIcon(_container)
                    {
                        Icon = Icon.ExtractAssociatedIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EnvironmentHelper.DesktopExecutableFileName)),
                        Text = "Remote Control Session",
                        BalloonTipIcon = ToolTipIcon.Info,
                        BalloonTipText = "A remote control session has started.",
                        BalloonTipTitle = "Remote Control Started",
                        ContextMenuStrip = _contextMenuStrip
                    };

                    _notifyIcon.Visible = true;
                    _notifyIcon.ShowBalloonTip(3000);
                }));
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private void App_Exit(object sender, EventArgs e)
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon?.Dispose();
                _notifyIcon?.Icon?.Dispose();
            }
        }
        private async void ExitMenuItem_Click(object sender, EventArgs e)
        {
            var casterSocket = ServiceContainer.Instance.GetRequiredService<ICasterSocket>();
            await casterSocket.DisconnectAllViewers();
        }
    }
}
