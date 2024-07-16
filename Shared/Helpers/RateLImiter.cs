using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace Immense.RemoteControl.Shared.Helpers;

public static class RateLimiter
{
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private static readonly SemaphoreSlim _cacheLock = new(1, 1);

    /// <summary>
    /// Clears the RateLimiter cache so it is reset to a fresh state.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task Reset(CancellationToken cancellationToken = default)
    {
        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            _cache.Clear();
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    /// <summary>
    /// <para>
    ///   Throttles a given func so it is only called once every duration.
    /// </para>
    /// <para>
    ///   The key used to identify and debounce the action will be derived from 
    ///   the caller information.  Those parameters will be populated automatically
    ///   and can be left empty.
    /// </para>
    /// </summary>
    public static async Task Throttle(
      Func<Task> func,
      TimeSpan duration,
      [CallerMemberName] string callerMemberName = "",
      [CallerFilePath] string callerFilePath = "",
      [CallerLineNumber] int callerLineNumber = -1,
      CancellationToken cancellationToken = default)
    {
        var key = $"{callerMemberName}-{callerFilePath}-{callerLineNumber}";
        await Throttle(func, duration, key, cancellationToken);
    }

    /// <summary>
    /// <para>
    ///   Throttles a given func so it is only called once every duration.
    /// </para>
    /// <para>
    ///   The provided key will be used to identify and throttle the func.
    /// </para>
    /// </summary>
    public static async Task Throttle(
      Func<Task> func,
      TimeSpan duration,
      string key,
      CancellationToken cancellationToken = default)
    {
        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            if (_cache.TryGetValue(key, out _))
            {
                return;
            }

            await func.Invoke();

            _cache.Set(key, string.Empty, duration);
        }
        finally
        {
            _cacheLock.Release();
        }
    }
}