using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Shared.Utilities;
using System;
using System.Diagnostics;
using System.IO;
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
                _ = Task.Run(() => WatchClipboard(cancelTokenSource.Token));
            }
        }

        public void SetText(string clipboardText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clipboardText))
                {
                    App.Current.Clipboard.ClearAsync().Wait();
                }
                else
                {
                    App.Current.Clipboard.SetTextAsync(clipboardText).Wait();
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

        private void WatchClipboard(CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested &&
                !Environment.HasShutdownStarted)
            {
                try
                {
                    var currentText = EnvironmentHelper.StartProcessWithResults("xclip", "-o");
                    // TODO: Switch back when fixed.
                    //var currentText = await App.Current.Clipboard.GetTextAsync();
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
