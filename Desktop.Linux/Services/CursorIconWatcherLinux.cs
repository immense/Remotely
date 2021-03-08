using Remotely.Desktop.Core.Interfaces;
using Remotely.Shared.Models;
using System;
using System.Drawing;

namespace Remotely.Desktop.Linux.Services
{
    public class CursorIconWatcherLinux : ICursorIconWatcher
    {
#pragma warning disable
        public event EventHandler<CursorInfo> OnChange;
#pragma warning restore

        public CursorInfo GetCurrentCursor() => new(null, Point.Empty, "default");
    }
}
