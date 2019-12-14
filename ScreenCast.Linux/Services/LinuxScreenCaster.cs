using Remotely.ScreenCast.Core;
using Remotely.ScreenCast.Core.Capture;
using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Linux.Capture;
using Remotely.Shared.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.ScreenCast.Linux.Services
{
    public class LinuxScreenCaster : IScreenCaster
    {
        public async Task BeginScreenCasting(ScreenCastRequest screenCastRequest)
        {
            try
            {
                var capturer = new X11Capture();
                await Conductor.Current.CasterSocket.SendCursorChange(new CursorInfo(null, Point.Empty, "default"), new List<string>() { screenCastRequest.ViewerID });
                _ = ScreenCaster.BeginScreenCasting(screenCastRequest.ViewerID, screenCastRequest.RequesterName, capturer);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
