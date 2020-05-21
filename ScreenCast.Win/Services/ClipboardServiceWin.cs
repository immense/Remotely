using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Services;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Remotely.ScreenCast.Win.Services
{
    public class ClipboardServiceWin : IClipboardService
    {
        public event EventHandler<string> ClipboardTextChanged;

        private string ClipboardText { get; set; }

        private System.Timers.Timer ClipboardWatcher { get; set; }

        public void BeginWatching()
        {
            ClipboardWatcher?.Dispose();
            ClipboardWatcher = new System.Timers.Timer(500);
            ClipboardWatcher.Elapsed += ClipboardWatcher_Elapsed;
            ClipboardWatcher.Start();
        }

        public void SetText(string clipboardText)
        {
            try
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(clipboardText))
                        {
                            Clipboard.Clear();
                        }
                        else
                        {
                            Clipboard.SetText(clipboardText);
                        }
                    }
                    catch { }
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

        private void ClipboardWatcher_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    Win32Interop.SwitchToInputDesktop();


                    if (Clipboard.ContainsText() && Clipboard.GetText() != ClipboardText)
                    {
                        ClipboardText = Clipboard.GetText();
                        ClipboardTextChanged?.Invoke(this, ClipboardText);
                    }
                }
                catch { }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
