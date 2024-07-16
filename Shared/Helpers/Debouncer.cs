using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Immense.RemoteControl.Shared.Helpers;

public static class Debouncer
{
    private static readonly ConcurrentDictionary<object, System.Timers.Timer> _timers = new();

    public static void Debounce(TimeSpan wait, Action action, [CallerMemberName] string key = "")
    {
        if (_timers.TryRemove(key, out var timer))
        {
            timer.Stop();
            timer.Dispose();
        }

        timer = new System.Timers.Timer(wait.TotalMilliseconds)
        {
            AutoReset = false
        };

        timer.Elapsed += (s, e) =>
        {
            try
            {
                action();
            }
            finally
            {
                if (_timers.TryGetValue(key, out var result))
                {
                    result?.Dispose();
                }
            }
        };
        _timers.TryAdd(key, timer);
        timer.Start();
    }
}
