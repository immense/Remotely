using Remotely.ScreenCast.Core.Capture;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Linux.X11Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace Remotely.ScreenCast.Linux.Capture
{
    public class X11Capture : ICapturer
    {
        public X11Capture()
        {
            Display = LibX11.XOpenDisplay(null);
            Init();
        }

        public bool CaptureFullscreen { get; set; }
        public Bitmap CurrentFrame { get; set; }
        public Rectangle CurrentScreenBounds { get; private set; }
        public IntPtr Display { get; private set; }
        public Bitmap PreviousFrame { get; set; }
        public event EventHandler<Rectangle> ScreenChanged;
        public int SelectedScreen { get; private set; } = -1;
        public void Capture()
        {
            try
            {
                PreviousFrame = (Bitmap)CurrentFrame.Clone();
                RefreshCurrentFrame();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Init();
            }
        }

        public void Dispose()
        {
            //Graphic.Dispose();
            CurrentFrame.Dispose();
            PreviousFrame.Dispose();
        }

        public int GetScreenCount()
        {
            return LibX11.XScreenCount(Display);
        }

        public Rectangle GetVirtualScreenBounds()
        {
            int width = 0;
            for (var i = 0; i < GetScreenCount(); i++)
            {
                width += LibX11.XWidthOfScreen(LibX11.XScreenOfDisplay(Display, i));
            }
            int height = 0;
            for (var i = 0; i < GetScreenCount(); i++)
            {
                height += LibX11.XHeightOfScreen(LibX11.XScreenOfDisplay(Display, i));
            }
            return new Rectangle(0, 0, width, height);
        }

        public void Init()
        {
            try
            {
                var defaultScreen = LibX11.XDefaultScreen(Display);
                SetSelectedScreen(defaultScreen);
                CurrentFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
                PreviousFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
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

        private void RefreshCurrentFrame()
        {
            var window = LibX11.XRootWindow(Display, SelectedScreen);

            var imagePointer = LibX11.XGetImage(Display, window, 0, 0, CurrentScreenBounds.Width, CurrentScreenBounds.Height, ~0, 2);
            var image = Marshal.PtrToStructure<LibX11.XImage>(imagePointer);

            var bd = CurrentFrame.LockBits(new Rectangle(0, 0, CurrentFrame.Width, CurrentFrame.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* scan1 = (byte*)bd.Scan0.ToPointer();
                byte* scan2 = (byte*)image.data.ToPointer();
                var bytesPerPixel = Bitmap.GetPixelFormatSize(CurrentFrame.PixelFormat) / 8;
                var totalSize = bd.Height * bd.Width * bytesPerPixel;
                for (int counter = 0; counter < totalSize - bytesPerPixel; counter++)
                {
                    scan1[counter] = scan2[counter];
                }
            }

            CurrentFrame.UnlockBits(bd);
            LibX11.XDestroyImage(imagePointer);
        }
    }
}
