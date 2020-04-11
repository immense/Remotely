using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Remotely.Shared.Helpers
{
    public static class TaskHelper
    {
        public static async Task<bool> DelayUntil(Func<bool> condition, TimeSpan timeout, int pollingMs = 10)
        {
            var sw = Stopwatch.StartNew();
            while (!condition() && sw.Elapsed < timeout)
            {
                await Task.Delay(pollingMs);
            }
            return condition();
        }
    }
}
