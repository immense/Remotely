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
        private CancellationTokenSource _cancelTokenSource;

        public event EventHandler<string> ClipboardTextChanged;

        private string ClipboardText { get; set; }

        public void BeginWatching()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                App.Current.Exit -= App_Exit;
                App.Current.Exit += App_Exit;
            });

            StopWatching();

            _cancelTokenSource = new CancellationTokenSource();


            WatchClipboard(_cancelTokenSource.Token);
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
            thread.IsBackground = true;
            thread.Start();

            return Task.CompletedTask;
        }

        public void StopWatching()
        {
            try
            {
                _cancelTokenSource?.Cancel();
                _cancelTokenSource?.Dispose();
            }
            catch { }
        }

        private void App_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            _cancelTokenSource?.Cancel();
        }
        private void WatchClipboard(CancellationToken cancelToken)
        {
            var thread = new Thread(() =>
            {

                while (!cancelToken.IsCancellationRequested)
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
                    Thread.Sleep(500);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
