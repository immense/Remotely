﻿using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Utilities;
using Remotely.Desktop.XPlat.Native.Linux;
using Remotely.Shared;
using Remotely.Shared.Utilities;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace Remotely.Desktop.XPlat.Services
{
    public class ScreenCapturerLinux : IScreenCapturer
    {
        private readonly object _screenBoundsLock = new();
        private readonly Dictionary<string, LibXrandr.XRRMonitorInfo> _x11Screens = new();
        private SKBitmap _currentFrame;
        private SKBitmap _previousFrame;

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
        public IEnumerable<string> GetDisplayNames()
        {
            return _x11Screens.Keys.Select(x => x.ToString());
        }

        public SKRect GetFrameDiffArea()
        {
            return ImageUtils.GetDiffArea(_currentFrame, _previousFrame, CaptureFullscreen);
        }

        public Result<SKBitmap> GetImageDiff()
        {
            return ImageUtils.GetImageDiff(_currentFrame, _previousFrame);
        }

        public Result<SKBitmap> GetNextFrame()
        {
            lock (_screenBoundsLock)
            {
                try
                {
                    if (_currentFrame != null)
                    {
                        _previousFrame?.Dispose();
                        try
                        {
                            _previousFrame = _currentFrame;
                        }
                        catch (Exception ex)
                        {
                            Logger.Write(ex);
                        }
                    }

                    _currentFrame = GetX11Capture();
                    return Result.Ok(_currentFrame);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                    Init();
                    return Result.Fail<SKBitmap>(ex);
                }
            }
        }
        public int GetScreenCount()
        {
            return _x11Screens.Count;
        }

        public int GetSelectedScreenIndex()
        {
            return int.Parse(SelectedScreen ?? "0");
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
                CaptureFullscreen = true;
                _x11Screens.Clear();

                var monitorsPtr = LibXrandr.XRRGetMonitors(Display, LibX11.XDefaultRootWindow(Display), true, out var monitorCount);

                var monitorInfoSize = Marshal.SizeOf<LibXrandr.XRRMonitorInfo>();

                for (var i = 0; i < monitorCount; i++)
                {
                    var monitorPtr = new IntPtr(monitorsPtr.ToInt64() + i * monitorInfoSize);
                    var monitorInfo = Marshal.PtrToStructure<LibXrandr.XRRMonitorInfo>(monitorPtr);

                    Logger.Write($"Found monitor: " +
                        $"{monitorInfo.width}," +
                        $"{monitorInfo.height}," +
                        $"{monitorInfo.x}, " +
                        $"{monitorInfo.y}");

                    _x11Screens.Add(i.ToString(), monitorInfo);
                }

                LibXrandr.XRRFreeMonitors(monitorsPtr);

                if (string.IsNullOrWhiteSpace(SelectedScreen) ||
                    !_x11Screens.ContainsKey(SelectedScreen))
                {
                    SelectedScreen = _x11Screens.Keys.First();
                    RefreshCurrentScreenBounds();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
        public void SetSelectedScreen(string displayName)
        {
            lock (_screenBoundsLock)
            {
                try
                {
                    Logger.Write($"Setting display to {displayName}.");
                    if (displayName == SelectedScreen)
                    {
                        return;
                    }
                    if (_x11Screens.ContainsKey(displayName))
                    {
                        SelectedScreen = displayName;
                    }
                    else
                    {
                        SelectedScreen = _x11Screens.Keys.First();
                    }

                    RefreshCurrentScreenBounds();

                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }
        }

        private SKBitmap GetX11Capture()
        {
            var currentFrame = new SKBitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height);

            var window = LibX11.XDefaultRootWindow(Display);

            var imagePointer = LibX11.XGetImage(Display,
                window,
                CurrentScreenBounds.X,
                CurrentScreenBounds.Y,
                CurrentScreenBounds.Width,
                CurrentScreenBounds.Height,
                ~0,
                2);

            var image = Marshal.PtrToStructure<LibX11.XImage>(imagePointer);

            var pixels = currentFrame.GetPixels();
            unsafe
            {
                byte* scan1 = (byte*)pixels.ToPointer();
                byte* scan2 = (byte*)image.data.ToPointer();
                var bytesPerPixel = currentFrame.BytesPerPixel;
                var totalSize = currentFrame.Height * currentFrame.Width * bytesPerPixel;
                for (int counter = 0; counter < totalSize - bytesPerPixel; counter++)
                {
                    scan1[counter] = scan2[counter];
                }
            }

            Marshal.DestroyStructure<LibX11.XImage>(imagePointer);
            LibX11.XDestroyImage(imagePointer);

            return currentFrame;
        }

        private void RefreshCurrentScreenBounds()
        {
            var screen = _x11Screens[SelectedScreen];

            Logger.Write($"Setting new screen bounds: " +
                 $"{screen.width}," +
                 $"{screen.height}," +
                 $"{screen.x}, " +
                 $"{screen.y}");

            CurrentScreenBounds = new Rectangle(screen.x, screen.y, screen.width, screen.height);
            CaptureFullscreen = true;
            ScreenChanged?.Invoke(this, CurrentScreenBounds);
        }
    }
}
