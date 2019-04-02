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
        public X11Capture(IntPtr display)
        {
            Display = display;
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
        public void Capture()
        {
            try
            {
                PreviousFrame = (Bitmap)CurrentFrame.Clone();
                Graphic.CopyFromScreen(CurrentScreenBounds.Left, CurrentScreenBounds.Top, 0, 0, new Size(CurrentScreenBounds.Width, CurrentScreenBounds.Height));
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
            return LibX11.XScreenCount(Display);
        }

        public double GetVirtualScreenHeight()
        {
            double height = 0;
            for (var i = 0; i < GetScreenCount(); i++)
            {
                height += LibX11.XHeightOfScreen(LibX11.XScreenOfDisplay(Display, i));
            }
            return height;
        }

        public double GetVirtualScreenWidth()
        {
            double width = 0;
            for (var i = 0; i < GetScreenCount(); i++)
            {
                width += LibX11.XWidthOfScreen(LibX11.XScreenOfDisplay(Display, i));
            }
            return width;
        }

        public void Init()
        {
            try
            {
                var defaultScreen = LibX11.XDefaultScreen(Display);
                SetSelectedScreen(defaultScreen);
                CurrentFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
                PreviousFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
                Graphic = Graphics.FromImage(CurrentFrame);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void SetSelectedScreen(int screenNumber)
        {
            if (screenNumber == SelectedScreen)
            {
                return;
            }
            try
            {
                if (GetScreenCount() >= screenNumber + 1)
                {
                    SelectedScreen = screenNumber;
                }
                else
                {
                    SelectedScreen = 0;
                }
                var width = LibX11.XDisplayWidth(Display, SelectedScreen);
                var height = LibX11.XDisplayHeight(Display, SelectedScreen);
                CurrentScreenBounds = new Rectangle(0, 0, width, height);
                CaptureFullscreen = true;
                Init();
                ScreenChanged?.Invoke(this, CurrentScreenBounds);

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
