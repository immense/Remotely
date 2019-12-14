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

namespace Remotely.ScreenCast.Win.Services
{
    public class WinScreenCaster : ScreenCasterBase,  IScreenCaster
    {
        public WinScreenCaster(CursorIconWatcher cursorIconWatcher, ICapturer capturer)
            : base(capturer)
        {
            CursorIconWatcher = cursorIconWatcher;
        }

        public CursorIconWatcher CursorIconWatcher { get; }

        public async Task BeginScreenCasting(ScreenCastRequest screenCastRequest)
        {
            await Conductor.Current.CasterSocket.SendCursorChange(CursorIconWatcher.GetCurrentCursor(), new List<string>() { screenCastRequest.ViewerID });
            _ = BeginScreenCasting(screenCastRequest.ViewerID, screenCastRequest.RequesterName, Capturer);
        }
    }
}
