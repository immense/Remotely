using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Core.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remotely.ScreenCast.Win.Services
{
    public class WinClipboardService : IClipboardService
    {
        public event EventHandler<string> ClipboardTextChanged;

        private string ClipboardText { get; set; }

        private System.Timers.Timer ClipboardWatcher { get; set; }

        public void BeginWatching()
        {
            try
            {
                if (ClipboardWatcher?.Enabled == true)
                {
                    ClipboardWatcher.Stop();
                }

                if (Clipboard.ContainsText())
                {
                    ClipboardText = Clipboard.GetText();
                    ClipboardTextChanged.Invoke(this, ClipboardText);
                }
                ClipboardWatcher = new System.Timers.Timer(500);
            }
            catch
            {
                return;
            }
            ClipboardWatcher.Elapsed += (sender, args) =>
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        if (Clipboard.ContainsText() && Clipboard.GetText() != ClipboardText)
                        {
                            ClipboardText = Clipboard.GetText();
                            ClipboardTextChanged.Invoke(this, ClipboardText);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            };
            ClipboardWatcher.Start();
        }

        public void SetText(string clipboardText)
        {
            try
            {
                var thread = new Thread(() =>
                {
                    Clipboard.SetText(clipboardText);
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
        public void StopWatching()
        {
            ClipboardWatcher?.Stop();
        }
    }
}
