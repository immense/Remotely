using Remotely.Desktop.Core.Interfaces;
using Remotely.Shared.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Desktop.Linux.Services
{
    public class ClipboardServiceLinux : IClipboardService
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
                _ = Task.Run(async () => await WatchClipboard(cancelTokenSource.Token));
            }
        }

        public async Task SetText(string clipboardText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clipboardText))
                {
                    await App.Current.Clipboard.ClearAsync();
                }
                else
                {
                    await App.Current.Clipboard.SetTextAsync(clipboardText);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void StopWatching()
        {
            cancelTokenSource?.Cancel();
            cancelTokenSource?.Dispose();
        }

        private async Task WatchClipboard(CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested &&
                !Environment.HasShutdownStarted)
            {
                try
                {
                    var currentText = await App.Current.Clipboard.GetTextAsync();
                    if (!string.IsNullOrEmpty(currentText) && currentText != ClipboardText)
                    {
                        ClipboardText = currentText;
                        ClipboardTextChanged?.Invoke(this, ClipboardText);
                    }
                }
                finally
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}
