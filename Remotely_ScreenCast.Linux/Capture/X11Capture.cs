using Remotely_ScreenCast.Core.Capture;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Remotely_ScreenCast.Linux.Capture
{
    public class X11Capture : ICapturer
    {
        public bool CaptureFullscreen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public System.Drawing.Bitmap CurrentFrame { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Rectangle CurrentScreenBounds => throw new NotImplementedException();

        public System.Drawing.Bitmap PreviousFrame { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public EventHandler<Rectangle> ScreenChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int SelectedScreen { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Capture()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int GetScreenCount()
        {
            throw new NotImplementedException();
        }

        public double GetVirtualScreenHeight()
        {
            throw new NotImplementedException();
        }

        public double GetVirtualScreenWidth()
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }
    }
}
