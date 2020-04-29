using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Linux.X11Interop;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace Remotely.ScreenCast.Linux.Services
{
    public class ScreenCapturerLinux : IScreenCapturer
    {
        private readonly Dictionary<string, int> x11Screens = new Dictionary<string, int>();
        public ScreenCapturerLinux()
        {
            Display = LibX11.XOpenDisplay(null);
            Init();
            GetNextFrame();
        }

        public event EventHandler<Rectangle> ScreenChanged;

        public bool CaptureFullscreen { get; set; }
        public Bitmap CurrentFrame { get; set; }
        public Rectangle CurrentScreenBounds { get; private set; }
        public IntPtr Display { get; private set; }
        public Bitmap PreviousFrame { get; set; }
        public string SelectedScreen { get; private set; }
        public void Dispose()
        {
            CurrentFrame.Dispose();
            PreviousFrame.Dispose();
        }

        public IEnumerable<string> GetDisplayNames() => x11Screens.Keys;

        public void GetNextFrame()
        {
            try
            {
                PreviousFrame?.Dispose();
                PreviousFrame = (Bitmap)CurrentFrame.Clone();
                RefreshCurrentFrame();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Init();
            }
        }
        public int GetScreenCount()
        {
            return LibX11.XScreenCount(Display);
        }

        public int GetSelectedScreenIndex() => x11Screens[SelectedScreen];

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
                x11Screens.Clear();

                for (var i = 0; i < GetScreenCount(); i++)
                {
                    x11Screens.Add(i.ToString(), i);
                }
                SetSelectedScreen(x11Screens.Keys.First());
                CurrentFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
                PreviousFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
        public void SetSelectedScreen(string displayName)
        {
            if (displayName == SelectedScreen)
            {
                return;
            }
            try
            {
                if (x11Screens.ContainsKey(displayName))
                {
                    SelectedScreen = displayName;
                }
                else
                {
                    SelectedScreen = x11Screens.Keys.First();
                }
                var width = LibX11.XDisplayWidth(Display, x11Screens[SelectedScreen]);
                var height = LibX11.XDisplayHeight(Display, x11Screens[SelectedScreen]);
                CurrentScreenBounds = new Rectangle(0, 0, width, height);
                CaptureFullscreen = true;
                ScreenChanged?.Invoke(this, CurrentScreenBounds);

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private void RefreshCurrentFrame()
        {
            var window = LibX11.XRootWindow(Display, x11Screens[SelectedScreen]);

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
