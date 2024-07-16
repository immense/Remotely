using System.Diagnostics;

namespace Immense.RemoteControl.Shared.Helpers;

public static class WaitHelper
{
    public static bool WaitFor(Func<bool> condition, TimeSpan timeout, int pollingMs = 10)
    {
        var sw = Stopwatch.StartNew();
        while (!condition() && sw.Elapsed < timeout)
        {
            Thread.Sleep(pollingMs);
        }
        return condition();
    }

    public static async Task<bool> WaitForAsync(Func<bool> condition, TimeSpan timeout, int pollingMs = 10)
    {
        var sw = Stopwatch.StartNew();
        while (!condition() && sw.Elapsed < timeout)
        {
            await Task.Delay(pollingMs).ConfigureAwait(false);
        }
        return condition();
    }
}
