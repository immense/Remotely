namespace Immense.RemoteControl.Shared.Services;

public interface ISystemTime
{
    DateTimeOffset Now { get; }

    DateTimeOffset Offset(TimeSpan offset);
    void Restore();
    void Set(DateTimeOffset time);
}

public class SystemTime : ISystemTime
{
    private TimeSpan _offset;
    private DateTimeOffset? _time;

    public DateTimeOffset Now
    {
        get
        {
            var baseTime = _time ?? DateTimeOffset.Now;
            return baseTime.Add(_offset);
        }
    }

    public DateTimeOffset Offset(TimeSpan offset)
    {
        _offset = _offset.Add(offset);
        return Now;
    }

    public void Restore()
    {
        _offset = TimeSpan.Zero;
        _time = null;
    }

    public void Set(DateTimeOffset time)
    {
        _offset = TimeSpan.Zero;
        _time = time;
    }
}
