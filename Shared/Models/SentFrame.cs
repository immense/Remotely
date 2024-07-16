namespace Immense.RemoteControl.Shared.Models;

public readonly struct SentFrame
{
    public SentFrame(int frameSize, DateTimeOffset timestamp)
    {
        FrameSize = frameSize;
        Timestamp = timestamp;
    }

    public DateTimeOffset Timestamp { get; }
    public int FrameSize { get; }
}
