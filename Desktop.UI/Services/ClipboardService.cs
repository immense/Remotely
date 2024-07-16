using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Immense.RemoteControl.Desktop.UI.Services;

public class ClipboardService : IClipboardService
{
    private readonly IUiDispatcher _dispatcher;
    private readonly ILogger<ClipboardService> _logger;
    private Task? _watcherTask;

    public event EventHandler<string>? ClipboardTextChanged;

    public ClipboardService(
        IUiDispatcher dispatcher,
        ILogger<ClipboardService> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    private string ClipboardText { get; set; } = string.Empty;

    public void BeginWatching()
    {
        if (_watcherTask?.Status == TaskStatus.Running)
        {
            return;
        }

        _watcherTask = Task.Run(
            async () => await WatchClipboard(_dispatcher.ApplicationExitingToken),
            _dispatcher.ApplicationExitingToken);
    }

    public async Task SetText(string clipboardText)
    {
        try
        {
            if (_dispatcher?.Clipboard is null)
            {
                _logger.LogWarning("Clipboard is null.");
                return;
            }

            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                await _dispatcher.Clipboard.ClearAsync();
            }
            else
            {
                await _dispatcher.Clipboard.SetTextAsync(clipboardText);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while setting text.");
        }
    }

    private async Task WatchClipboard(CancellationToken cancelToken)
    {
        while (
            !cancelToken.IsCancellationRequested &&
            !Environment.HasShutdownStarted)
        {
            try
            {
                if (_dispatcher?.Clipboard is null)
                {
                    continue;
                }

                var currentText = await _dispatcher.Clipboard.GetTextAsync();
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
