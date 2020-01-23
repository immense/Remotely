using Remotely.ScreenCast.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remotely.ScreenCast.Core.Enums;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Core.Capture;
using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Models;
using Remotely.Shared.Models;
using Remotely.ScreenCast.Win.Capture;
using Remotely.Shared.Win32;
using Microsoft.Extensions.DependencyInjection;

namespace Remotely.ScreenCast.Win.Services
{
    public class WinScreenCaster : ScreenCasterBase,  IScreenCaster
    {
        public WinScreenCaster(CursorIconWatcher cursorIconWatcher)
        {
            CursorIconWatcher = cursorIconWatcher;
        }

        public CursorIconWatcher CursorIconWatcher { get; }

        public async Task BeginScreenCasting(ScreenCastRequest screenCastRequest)
        {
            if (Win32Interop.GetCurrentDesktop(out var currentDesktopName))
            {
                Logger.Write($"Setting desktop to {currentDesktopName} before screen casting.");
                Win32Interop.SwitchToInputDesktop();
            }
            else
            {
                Logger.Write("Failed to get current desktop before screen casting.");
            }

            var conductor = ServiceContainer.Instance.GetRequiredService<Conductor>();
            await conductor.CasterSocket.SendCursorChange(CursorIconWatcher.GetCurrentCursor(), new List<string>() { screenCastRequest.ViewerID });
            _ = BeginScreenCasting(screenCastRequest.ViewerID, screenCastRequest.RequesterName, ServiceContainer.Instance.GetRequiredService<ICapturer>());
        }

    }
}
