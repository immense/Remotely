using Remotely.Desktop.Core.Interfaces;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remotely.Desktop.Win.Services
{
    public class ClipboardServiceWin : IClipboardService
    {

        private CancellationTokenSource cancelTokenSource;

        public event EventHandler<string> ClipboardTextChanged;

        private string ClipboardText { get; set; }

        public void BeginWatching()
        {
            try
            {
                StopWatching();
            }
            finally
            {
                cancelTokenSource = new CancellationTokenSource();
                _ = Task.Run(() => WatchClipboard(cancelTokenSource.Token));
            }
        }

        public Task SetText(string clipboardText)
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
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return Task.CompletedTask;
        }

        public void StopWatching()
        {
            try
            {
                cancelTokenSource?.Cancel();
                cancelTokenSource?.Dispose();
            }
            catch { }
        }

        private void WatchClipboard(CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested &&
                !Environment.HasShutdownStarted)
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
                Thread.Sleep(500);
            }
        }
    }
}
