using Remotely.ScreenCast.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Core;
using Remotely.Shared.Models;
using Remotely.Shared.Win32;
using Microsoft.Extensions.DependencyInjection;
using Remotely.ScreenCast.Core.Models;

namespace Remotely.ScreenCast.Win.Services
{
    public class ScreenCasterWin : ScreenCasterBase,  IScreenCaster
    {
        public ScreenCasterWin(CursorIconWatcher cursorIconWatcher)
        {
            CursorIconWatcher = cursorIconWatcher;
        }

        protected CursorIconWatcher CursorIconWatcher { get; }

        public async Task BeginScreenCasting(ScreenCastRequest screenCastRequest)
        {
            try
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
                _ = BeginScreenCasting(screenCastRequest.ViewerID, screenCastRequest.RequesterName);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
