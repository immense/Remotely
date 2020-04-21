using Remotely.ScreenCast.Core.Interfaces;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Remotely.ScreenCast.Linux.Services
{
    public class CursorIconWatcherLinux : ICursorIconWatcher
    {
#pragma warning disable
        public event EventHandler<CursorInfo> OnChange;
#pragma warning restore

        public CursorInfo GetCurrentCursor() => new CursorInfo(null, Point.Empty, "default");
    }
}
