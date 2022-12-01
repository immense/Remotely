// The DirectX capture code is based off examples from the
// SharpDX Samples at https://github.com/sharpdx/SharpDX.

// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using Microsoft.Win32;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Utilities;
using Remotely.Desktop.Win.Models;
using Remotely.Shared.Utilities;
using Remotely.Shared.Win32;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Remotely.Shared;
using Result = Remotely.Shared.Result;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Remotely.Desktop.Core.Extensions;
using System.Runtime.InteropServices;

namespace Remotely.Desktop.Win.Services
{
    public class ScreenCapturerWin : IScreenCapturer
    {
        private readonly Dictionary<string, int> _bitBltScreens = new();
        private readonly Dictionary<string, DirectXOutput> _directxScreens = new();
        private readonly object _screenBoundsLock = new();
        private SKBitmap _currentFrame;
        private SKBitmap _previousFrame;

        public ScreenCapturerWin()
        {
            Init();
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        public event EventHandler<Rectangle> ScreenChanged;

        public bool CaptureFullscreen { get; set; } = true;
        public Rectangle CurrentScreenBounds { get; private set; } = Screen.PrimaryScreen.Bounds;
        public bool NeedsInit { get; set; } = true;
        public string SelectedScreen { get; private set; } = Screen.PrimaryScreen.DeviceName;
        public void Dispose()
        {
            try
            {
                SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
                ClearDirectXOutputs();
                GC.SuppressFinalize(this);
            }
            catch { }
        }
        public IEnumerable<string> GetDisplayNames()
        {
            return Screen.AllScreens.Select(x => x.DeviceName);
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
                    if (!Win32Interop.SwitchToInputDesktop())
                    {
                        // Something will occasionally prevent this from succeeding after active
                        // desktop has changed to/from WinLogon (err code 170).  I'm guessing a hook
                        // is getting put in the desktop, which causes SetThreadDesktop to fail.
                        // The caller can start a new thread, which seems to resolve it.
                        var errCode = Marshal.GetLastWin32Error();
                        var errMessage = $"Failed to switch to input desktop. Last Win32 error code: {errCode}";
                        Logger.Write(errMessage);
                        return Result.Fail<SKBitmap>(errMessage);
                    }

                    if (NeedsInit)
                    {
                        Logger.Write("Init needed in GetNextFrame.");
                        Init();
                    }

                    SwapFrames();

                    var result = GetDirectXFrame();

                    if (!result.IsSuccess || result.Value is null || IsEmpty(result.Value))
                    {
                        result = GetBitBltFrame();
                        if (!result.IsSuccess || result.Value is null)
                        {
                            var ex = result.Exception ?? new("Unknown error.");
                            Logger.Write(ex);
                            return Result.Fail<SKBitmap>(ex);
                        }
                    }

                    _currentFrame = result.Value;
                    return result;
                }
                catch (Exception e)
                {
                    Logger.Write(e);
                    NeedsInit = true;
                    return Result.Fail<SKBitmap>(e);
                }
            }
        }

        public int GetScreenCount()
        {
            return Screen.AllScreens.Length;
        }

        public int GetSelectedScreenIndex()
        {
            if (_bitBltScreens.TryGetValue(SelectedScreen, out var index))
            {
                return index;
            }
            return 0;
        }

        public Rectangle GetVirtualScreenBounds()
        {
            return SystemInformation.VirtualScreen;
        }

        public void Init()
        {
            Win32Interop.SwitchToInputDesktop();

            CaptureFullscreen = true;
            InitBitBlt();
            InitDirectX();

            ScreenChanged?.Invoke(this, CurrentScreenBounds);

            NeedsInit = false;
        }

        public void SetSelectedScreen(string displayName)
        {
            lock (_screenBoundsLock)
            {
                if (displayName == SelectedScreen)
                {
                    return;
                }

                if (_bitBltScreens.ContainsKey(displayName))
                {
                    SelectedScreen = displayName;
                }
                else
                {
                    SelectedScreen = _bitBltScreens.Keys.First();
                }
                RefreshCurrentScreenBounds();
            }
        }

        internal Result<SKBitmap> GetBitBltFrame()
        {
            try
            {
                using var bitmap = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
                using (var graphic = Graphics.FromImage(bitmap))
                {
                    graphic.CopyFromScreen(CurrentScreenBounds.Left, CurrentScreenBounds.Top, 0, 0, new Size(CurrentScreenBounds.Width, CurrentScreenBounds.Height));
                }
                return Result.Ok(bitmap.ToSKBitmap());
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Logger.Write("Capturer error in BitBltCapture.");
                NeedsInit = true;
                return Result.Fail<SKBitmap>("Error while capturing BitBlt frame.");
            }
        }

        internal Result<SKBitmap> GetDirectXFrame()
        {
            if (!_directxScreens.TryGetValue(SelectedScreen, out var dxOutput))
            {
                return Result.Fail<SKBitmap>("DirectX output not found.");
            }

            try
            {
                var outputDuplication = dxOutput.OutputDuplication;
                var device = dxOutput.Device;
                var texture2D = dxOutput.Texture2D;
                var bounds = dxOutput.Bounds;

                var result = outputDuplication.TryAcquireNextFrame(50, out var duplicateFrameInfo, out var screenResource);

                if (!result.Success)
                {
                    return Result.Fail<SKBitmap>("Next frame did not arrive.");
                }

                if (duplicateFrameInfo.AccumulatedFrames == 0)
                {
                    try
                    {
                        outputDuplication.ReleaseFrame();
                    }
                    catch { }
                    return Result.Fail<SKBitmap>("No frames were accumulated.");
                }

                using Texture2D screenTexture2D = screenResource.QueryInterface<Texture2D>();
                device.ImmediateContext.CopyResource(screenTexture2D, texture2D);
                var dataBox = device.ImmediateContext.MapSubresource(texture2D, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);
                using var bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
                var bitmapData = bitmap.LockBits(bounds, ImageLockMode.WriteOnly, bitmap.PixelFormat);
                var dataBoxPointer = dataBox.DataPointer;
                var bitmapDataPointer = bitmapData.Scan0;
                for (var y = 0; y < bounds.Height; y++)
                {
                    Utilities.CopyMemory(bitmapDataPointer, dataBoxPointer, bounds.Width * 4);
                    dataBoxPointer = IntPtr.Add(dataBoxPointer, dataBox.RowPitch);
                    bitmapDataPointer = IntPtr.Add(bitmapDataPointer, bitmapData.Stride);
                }
                bitmap.UnlockBits(bitmapData);
                device.ImmediateContext.UnmapSubresource(texture2D, 0);
                screenResource?.Dispose();

                switch (dxOutput.Rotation)
                {
                    case DisplayModeRotation.Unspecified:
                    case DisplayModeRotation.Identity:
                        break;
                    case DisplayModeRotation.Rotate90:
                        bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    case DisplayModeRotation.Rotate180:
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case DisplayModeRotation.Rotate270:
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    default:
                        break;
                }
                return Result.Ok(bitmap.ToSKBitmap());
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error while getting DirectX frame.");
            }
            finally
            {
                try
                {
                    dxOutput.OutputDuplication.ReleaseFrame();
                }
                catch { }
            }

            return Result.Fail<SKBitmap>("Failed to get DirectX frame.");
        }

        private void ClearDirectXOutputs()
        {
            foreach (var screen in _directxScreens.Values)
            {
                try
                {
                    screen.Dispose();
                }
                catch { }
            }
            _directxScreens.Clear();
        }
        private void InitBitBlt()
        {
            _bitBltScreens.Clear();
            for (var i = 0; i < Screen.AllScreens.Length; i++)
            {
                _bitBltScreens.Add(Screen.AllScreens[i].DeviceName, i);
            }
        }

        private void InitDirectX()
        {
            try
            {
                ClearDirectXOutputs();

                using var factory = new Factory1();
                foreach (var adapter in factory.Adapters1.Where(x => (x.Outputs?.Length ?? 0) > 0))
                {
                    foreach (var output in adapter.Outputs)
                    {
                        try
                        {
                            var device = new SharpDX.Direct3D11.Device(adapter);
                            var output1 = output.QueryInterface<Output1>();

                            var bounds = output1.Description.DesktopBounds;
                            var width = bounds.Right - bounds.Left;
                            var height = bounds.Bottom - bounds.Top;

                            // Create Staging texture CPU-accessible
                            var textureDesc = new Texture2DDescription
                            {
                                CpuAccessFlags = CpuAccessFlags.Read,
                                BindFlags = BindFlags.None,
                                Format = Format.B8G8R8A8_UNorm,
                                Width = width,
                                Height = height,
                                OptionFlags = ResourceOptionFlags.None,
                                MipLevels = 1,
                                ArraySize = 1,
                                SampleDescription = { Count = 1, Quality = 0 },
                                Usage = ResourceUsage.Staging
                            };

                            var texture2D = new Texture2D(device, textureDesc);

                            _directxScreens.Add(
                                output1.Description.DeviceName,
                                new DirectXOutput(adapter,
                                    device,
                                    output1.DuplicateOutput(device),
                                    texture2D,
                                    output1.Description.Rotation));
                        }
                        catch (Exception ex)
                        {
                            Logger.Write(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private bool IsEmpty(SKBitmap bitmap)
        {
            if (bitmap is null)
            {
                return true;
            }

            var height = bitmap.Height;
            var width = bitmap.Width;
            var bytesPerPixel = bitmap.BytesPerPixel;

            try
            {
                unsafe
                {
                    byte* scan = (byte*)bitmap.GetPixels();

                    for (var row = 0; row < height; row++)
                    {
                        for (var column = 0; column < width; column++)
                        {
                            var index = (row * width * bytesPerPixel) + (column * bytesPerPixel);

                            byte* data = scan + index;

                            for (var i = 0; i < bytesPerPixel; i++)
                            {
                                if (data[i] != 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }

                    return true;
                }
            }
            catch
            {
                return true;
            }
        }

        private void RefreshCurrentScreenBounds()
        {
            CurrentScreenBounds = Screen.AllScreens[_bitBltScreens[SelectedScreen]].Bounds;
            CaptureFullscreen = true;
            NeedsInit = true;
            ScreenChanged?.Invoke(this, CurrentScreenBounds);
        }

        private void SwapFrames()
        {
            if (_currentFrame != null)
            {
                _previousFrame?.Dispose();
                _previousFrame = _currentFrame;
            }
        }
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            RefreshCurrentScreenBounds();
        }
    }
}
