using Remotely_ScreenCast.Core.Capture;
using Remotely_ScreenCast.Core.Utilities;
using Remotely_ScreenCast.Linux.X11Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Remotely_ScreenCast.Linux.Capture
{
    public class X11Capture : ICapturer
    {
        public X11Capture()
        {
            Init();
        }

        public bool CaptureFullscreen { get; set; }
        public Bitmap CurrentFrame { get; set; }
        public Rectangle CurrentScreenBounds { get; private set; }
        public IntPtr Display { get; private set; }
        public Bitmap PreviousFrame { get; set; }
        public EventHandler<Rectangle> ScreenChanged { get; set; }
        public int SelectedScreen { get; private set; } = -1;
        private Graphics Graphic { get; set; }
        private object ScreenLock { get; } = new object();
        public void Capture()
        {
            try
            {
                lock (ScreenLock)
                {
                    PreviousFrame = (Bitmap)CurrentFrame.Clone();
                    Graphic.CopyFromScreen(CurrentScreenBounds.Left, CurrentScreenBounds.Top, 0, 0, new Size(CurrentScreenBounds.Width, CurrentScreenBounds.Height));
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Init();
            }
        }

        public void Dispose()
        {
            Graphic.Dispose();
            CurrentFrame.Dispose();
            PreviousFrame.Dispose();
        }

        public int GetScreenCount()
        {
            return Xlib.XScreenCount(Display);
        }

        public double GetVirtualScreenHeight()
        {
            double height = 0;
            for (var i = 0; i < GetScreenCount(); i++)
            {
                height += Xlib.XHeightOfScreen(Xlib.XScreenOfDisplay(Display, i));
            }
            return height;
        }

        public double GetVirtualScreenWidth()
        {
            double width = 0;
            for (var i = 0; i < GetScreenCount(); i++)
            {
                width += Xlib.XWidthOfScreen(Xlib.XScreenOfDisplay(Display, i));
            }
            return width;
        }

        public void Init()
        {
            Display = Xlib.XOpenDisplay(null);
            var defaultScreen = Xlib.XDefaultScreen(Display);
            SetSelectedScreen(defaultScreen);
            CurrentFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
            PreviousFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
            Graphic = Graphics.FromImage(CurrentFrame);
        }

        public void SetSelectedScreen(int screenNumber)
        {
            if (screenNumber == SelectedScreen)
            {
                return;
            }
            lock (ScreenLock)
            {
                if (GetScreenCount() >= screenNumber + 1)
                {
                    SelectedScreen = screenNumber;
                }
                else
                {
                    SelectedScreen = 0;
                }
                var width = Xlib.XDisplayWidth(Display, SelectedScreen);
                var height = Xlib.XDisplayHeight(Display, SelectedScreen);
                CurrentScreenBounds = new Rectangle(0, 0, width, height);
                CaptureFullscreen = true;
                Init();
                ScreenChanged?.Invoke(this, CurrentScreenBounds);
            }
        }
    }
}
