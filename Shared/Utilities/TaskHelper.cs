using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Shared.Utilities
{
    public static class TaskHelper
    {
        public static bool DelayUntil(Func<bool> condition, TimeSpan timeout, int pollingMs = 10)
        {
            var sw = Stopwatch.StartNew();
            while (!condition() && sw.Elapsed < timeout)
            {
                Thread.Sleep(pollingMs);
            }
            return condition();
        }

        public static Task<bool> DelayUntilAsync(Func<bool> condition, TimeSpan timeout, int pollingMs = 10)
        {
            return Task.Run(() =>
            {
                var sw = Stopwatch.StartNew();
                while (!condition() && sw.Elapsed < timeout)
                {
                    Thread.Sleep(pollingMs);
                }
                return condition();
            });
        }
    }
}
