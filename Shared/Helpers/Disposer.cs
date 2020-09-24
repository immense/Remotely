using System;

namespace Remotely.Shared.Helpers
{
    public static class Disposer
    {
        public static void TryDisposeAll(IDisposable[] disposables)
        {
            if (disposables is null)
            {
                return;
            }

            foreach (var disposable in disposables)
            {
                try
                {
                    disposable?.Dispose();
                }
                catch { }
            }
        }
    }
}
