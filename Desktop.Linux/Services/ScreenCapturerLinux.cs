using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Linux.Native;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Remotely.Desktop.Linux.Services
{
    public class ScreenCapturerLinux : IScreenCapturer
    {
        private readonly SemaphoreSlim _screenCaptureLock = new(1,1);
        private readonly Dictionary<string, int> _x11Screens = new();
        public ScreenCapturerLinux()
        {
            Display = LibX11.XOpenDisplay(null);
            Init();
        }

        public event EventHandler<Rectangle> ScreenChanged;

        public bool CaptureFullscreen { get; set; } = true;
        public Rectangle CurrentScreenBounds { get; private set; }
        public IntPtr Display { get; private set; }
        public string SelectedScreen { get; private set; }

        public void Dispose()
        {
            LibX11.XCloseDisplay(Display);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<string> GetDisplayNames() => _x11Screens.Keys;

        public Bitmap GetNextFrame()
        {
            try
            {
                _screenCaptureLock.Wait();

                return GetX11Screen();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Init();
                return null;
            }
            finally
            {
                _screenCaptureLock.Release();
            }
        }
        public int GetScreenCount()
        {
            return LibX11.XScreenCount(Display);
        }

        public int GetSelectedScreenIndex() => _x11Screens[SelectedScreen];

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
                CaptureFullscreen = true;
                _x11Screens.Clear();

                for (var i = 0; i < GetScreenCount(); i++)
                {
                    _x11Screens.Add(i.ToString(), i);
                }
                SetSelectedScreen(_x11Screens.Keys.First());
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
                if (_x11Screens.ContainsKey(displayName))
                {
                    SelectedScreen = displayName;
                }
                else
                {
                    SelectedScreen = _x11Screens.Keys.First();
                }
                var width = LibX11.XDisplayWidth(Display, _x11Screens[SelectedScreen]);
                var height = LibX11.XDisplayHeight(Display, _x11Screens[SelectedScreen]);
                CurrentScreenBounds = new Rectangle(0, 0, width, height);
                CaptureFullscreen = true;
                ScreenChanged?.Invoke(this, CurrentScreenBounds);

            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private Bitmap GetX11Screen()
        {
            var currentFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);

            var window = LibX11.XRootWindow(Display, _x11Screens[SelectedScreen]);

            var imagePointer = LibX11.XGetImage(Display, window, 0, 0, CurrentScreenBounds.Width, CurrentScreenBounds.Height, ~0, 2);
            var image = Marshal.PtrToStructure<LibX11.XImage>(imagePointer);

            var bd = currentFrame.LockBits(new Rectangle(0, 0, CurrentScreenBounds.Width, CurrentScreenBounds.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* scan1 = (byte*)bd.Scan0.ToPointer();
                byte* scan2 = (byte*)image.data.ToPointer();
                var bytesPerPixel = Bitmap.GetPixelFormatSize(currentFrame.PixelFormat) / 8;
                var totalSize = bd.Height * bd.Width * bytesPerPixel;
                for (int counter = 0; counter < totalSize - bytesPerPixel; counter++)
                {
                    scan1[counter] = scan2[counter];
                }
            }

            currentFrame.UnlockBits(bd);
            LibX11.XDestroyImage(imagePointer);

            return currentFrame;
        }
    }
}
