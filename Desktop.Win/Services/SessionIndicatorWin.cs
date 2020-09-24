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
        private Container container;
        private ContextMenuStrip contextMenuStrip;
        private NotifyIcon notifyIcon;
        public SessionIndicatorWin(Form backgroundForm)
        {
            BackgroundForm = backgroundForm;
            Application.ApplicationExit += Application_ApplicationExit;
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            notifyIcon?.Dispose();
        }

        private Form BackgroundForm { get; }

        public void Show()
        {
            try
            {
                if (notifyIcon != null)
                {
                    return;
                }

                BackgroundForm.Invoke(new Action(() =>
                {
                    container = new Container();
                    contextMenuStrip = new ContextMenuStrip(container);
                    contextMenuStrip.Items.Add("Exit", null, ExitMenuItem_Click);

                    notifyIcon = new NotifyIcon(container)
                    {
                        Icon = Icon.ExtractAssociatedIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EnvironmentHelper.DesktopExecutableFileName)),
                        Text = "Remote Control Session",
                        BalloonTipIcon = ToolTipIcon.Info,
                        BalloonTipText = "A remote control session has started.",
                        BalloonTipTitle = "Remote Control Started",
                        ContextMenuStrip = contextMenuStrip
                    };

                    notifyIcon.Visible = true;
                    notifyIcon.ShowBalloonTip(3000);
                }));
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private async void ExitMenuItem_Click(object sender, EventArgs e)
        {
            var casterSocket = ServiceContainer.Instance.GetRequiredService<CasterSocket>();
            await casterSocket.DisconnectAllViewers();
        }
    }
}
