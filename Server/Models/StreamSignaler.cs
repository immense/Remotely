namespace Immense.RemoteControl.Server.Models;

public class StreamSignaler : IDisposable
{
    private bool _disposedValue;

    public StreamSignaler(Guid streamId)
    {
        StreamId = streamId;
    }

    public string DesktopConnectionId { get; internal set; } = string.Empty;
    public SemaphoreSlim ReadySignal { get; } = new(0, 1);
    public SemaphoreSlim EndSignal { get; } = new(0, 1);
    public Guid StreamId { get; init; }
    public string ViewerConnectionId { get; internal set; } = string.Empty;
    public IAsyncEnumerable<byte[]>? Stream { get; internal set; }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                EndSignal.Dispose();
                ReadySignal.Dispose();
            }
            _disposedValue = true;
        }
    }
}
