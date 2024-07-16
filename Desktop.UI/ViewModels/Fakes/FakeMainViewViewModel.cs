using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.Shared.Reactive;
using Immense.RemoteControl.Desktop.Shared.Services;
using Immense.RemoteControl.Desktop.Shared.ViewModels;
using Immense.RemoteControl.Shared.Models;
using Immense.RemoteControl.Shared.Models.Dtos;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;

namespace Immense.RemoteControl.Desktop.UI.ViewModels.Fakes;
public class FakeMainViewViewModel : FakeBrandedViewModelBase, IMainViewViewModel
{

    public AsyncRelayCommand ChangeServerCommand => new(() => Task.CompletedTask);

    public ICommand CloseCommand => new RelayCommand(() => { });
    public AsyncRelayCommand CopyLinkCommand => new(() => Task.CompletedTask);
    public double CopyMessageOpacity { get; set; }

    public string Host { get; set; } = string.Empty;

    public bool IsAdministrator => true;

    public bool IsCopyMessageVisible { get; set; }
    public ICommand MinimizeCommand => new RelayCommand(() => { });
    public ICommand OpenOptionsMenu => new RelayCommand(() => { });
    public AsyncRelayCommand RemoveViewersCommand => new(() => Task.CompletedTask);

    public string StatusMessage { get; set; } = "392 527 094";

    public ObservableCollection<IViewer> Viewers { get; } = new() { new FakeViewer() };

    public IList<IViewer> SelectedViewers { get; } = new List<IViewer>();

    public bool CanRemoveViewers()
    {
        return true;
    }

    public Task ChangeServer()
    {
        throw new NotImplementedException();
    }

    public Task CopyLink()
    {
        return Task.CompletedTask;
    }

    public Task GetSessionID()
    {
        return Task.CompletedTask;
    }

    public Task Init()
    {
        return Task.CompletedTask;
    }

    public Task PromptForHostName()
    {
        return Task.CompletedTask;
    }

    public Task RemoveViewers()
    {
        return Task.CompletedTask;
    }

    private class FakeViewer : IViewer
    {
        public IScreenCapturer Capturer => null!;

        public double CurrentFps => default;

        public double CurrentMbps => default;

        public bool DisconnectRequested { get; set; } = false;
        public bool HasControl { get; set; } = true;

        public int ImageQuality => 80;

        public bool IsResponsive => true;

        public string Name { get; set; } = "Rick James";

        public TimeSpan RoundTripLatency => default;

        public string ViewerConnectionId { get; set; } = string.Empty;

        public void AppendSentFrame(SentFrame sentFrame)
        {
        }

        public Task ApplyAutoQuality()
        {
            return Task.CompletedTask;
        }

        public Task CalculateMetrics()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public void IncrementFpsCount()
        {
            
        }

        public Task SendAudioSample(byte[] audioSample)
        {
            return Task.CompletedTask;
        }

        public Task SendClipboardText(string clipboardText)
        {
            return Task.CompletedTask;
        }

        public Task SendCursorChange(CursorInfo cursorInfo)
        {
            return Task.CompletedTask;
        }

        public Task SendDesktopStream(IAsyncEnumerable<byte[]> asyncEnumerable, Guid streamId)
        {
            return Task.CompletedTask;
        }

        public Task SendFile(FileUpload fileUpload, Action<double> progressUpdateCallback, CancellationToken cancelToken)
        {
            return Task.CompletedTask;
        }

        public Task SendScreenData(string selectedDisplay, IEnumerable<string> displayNames, int screenWidth, int screenHeight)
        {
            return Task.CompletedTask;
        }

        public Task SendScreenSize(int width, int height)
        {
            return Task.CompletedTask;
        }

        public Task SendSessionMetrics(SessionMetricsDto metrics)
        {
            return Task.CompletedTask;
        }

        public Task SendWindowsSessions()
        {
            return Task.CompletedTask;
        }

        public void SetLastFrameReceived(DateTimeOffset timestamp)
        {
        }

        public Task<bool> WaitForViewer()
        {
            return Task.FromResult(true);
        }
    }
}
