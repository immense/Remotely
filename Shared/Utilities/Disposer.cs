using System;

namespace Remotely.Shared.Utilities
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
