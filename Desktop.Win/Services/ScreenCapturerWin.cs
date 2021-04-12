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

namespace Remotely.Desktop.Win.Services
{
    public class ScreenCapturerWin : IScreenCapturer
    {
        private readonly Dictionary<string, int> _bitBltScreens = new();
        private readonly Dictionary<string, DirectXOutput> _directxScreens = new();
        private readonly object _screenBoundsLock = new();

        public ScreenCapturerWin()
        {
            Init();
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        public event EventHandler<Rectangle> ScreenChanged;

        private enum GetDirectXFrameResult
        {
            Success,
            Failure,
            Timeout,
        }

        public bool CaptureFullscreen { get; set; } = true;
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
        public Rectangle CurrentScreenBounds { get; private set; } = Screen.PrimaryScreen.Bounds;

        public IEnumerable<string> GetDisplayNames() => Screen.AllScreens.Select(x => x.DeviceName);

        public Bitmap GetNextFrame()
        {
            lock (_screenBoundsLock)
            {
                try
                {

                    Win32Interop.SwitchToInputDesktop();

                    if (NeedsInit)
                    {
                        Logger.Write("Init needed in GetNextFrame.");
                        Init();
                    }

                    // Sometimes DX will result in a timeout, even when there are changes
                    // on the screen.  I've observed this when a laptop lid is closed, or
                    // on some machines that aren't connected to a monitor.  This will
                    // have it fall back to BitBlt in those cases.
                    // TODO: Make DX capture work with changed screen orientation.
                    if (_directxScreens.ContainsKey(SelectedScreen) &&
                        SystemInformation.ScreenOrientation != ScreenOrientation.Angle270 &&
                        SystemInformation.ScreenOrientation != ScreenOrientation.Angle90)
                    {
                        var (result, frame) = GetDirectXFrame();

                        if (result == GetDirectXFrameResult.Success)
                        {
                            return frame;
                        }
                    }

                    return GetBitBltFrame();

                }
                catch (Exception e)
                {
                    Logger.Write(e);
                    NeedsInit = true;
                }
                return null;
            }

        }

        public int GetScreenCount() => Screen.AllScreens.Length;

        public int GetSelectedScreenIndex()
        {
            if (_bitBltScreens.TryGetValue(SelectedScreen, out var index))
            {
                return index;
            }
            return 0;
        }

        public Rectangle GetVirtualScreenBounds() => SystemInformation.VirtualScreen;

        public void Init()
        {
            Win32Interop.SwitchToInputDesktop();

            CaptureFullscreen = true;
            InitBitBlt();
            InitDirectX();

            ScreenChanged?.Invoke(this, CurrentScreenBounds);
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

        private Bitmap GetBitBltFrame()
        {
            try
            {
                var currentFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
                using (var graphic = Graphics.FromImage(currentFrame))
                {
                    graphic.CopyFromScreen(CurrentScreenBounds.Left, CurrentScreenBounds.Top, 0, 0, new Size(CurrentScreenBounds.Width, CurrentScreenBounds.Height));
                }
                return currentFrame;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Logger.Write("Capturer error in BitBltCapture.");
                NeedsInit = true;
            }

            return null;
        }
        private (GetDirectXFrameResult result, Bitmap frame) GetDirectXFrame()
        {
            try
            {
                var duplicatedOutput = _directxScreens[SelectedScreen].OutputDuplication;
                var device = _directxScreens[SelectedScreen].Device;
                var texture2D = _directxScreens[SelectedScreen].Texture2D;

                // Try to get duplicated frame within given time is ms
                var result = duplicatedOutput.TryAcquireNextFrame(100,
                    out var duplicateFrameInformation,
                    out var screenResource);

                if (result.Failure)
                {
                    if (result.Code == SharpDX.DXGI.ResultCode.WaitTimeout.Code)
                    {
                        return (GetDirectXFrameResult.Timeout, null);
                    }
                    else
                    {
                        Logger.Write($"TryAcquireFrame error.  Code: {result.Code}");
                        NeedsInit = true;
                        return (GetDirectXFrameResult.Failure, null);
                    }
                }

                if (duplicateFrameInformation.AccumulatedFrames == 0)
                {
                    try
                    {
                        duplicatedOutput.ReleaseFrame();
                    }
                    catch { }
                    return (GetDirectXFrameResult.Failure, null);
                }

                var currentFrame = new Bitmap(texture2D.Description.Width, texture2D.Description.Height, PixelFormat.Format32bppArgb);

                // Copy resource into memory that can be accessed by the CPU
                using (var screenTexture2D = screenResource.QueryInterface<Texture2D>())
                {
                    device.ImmediateContext.CopyResource(screenTexture2D, texture2D);
                }

                // Get the desktop capture texture
                var mapSource = device.ImmediateContext.MapSubresource(texture2D, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

                var boundsRect = new Rectangle(0, 0, texture2D.Description.Width, texture2D.Description.Height);

                // Copy pixels from screen capture Texture to GDI bitmap
                var mapDest = currentFrame.LockBits(boundsRect, ImageLockMode.WriteOnly, currentFrame.PixelFormat);
                var sourcePtr = mapSource.DataPointer;
                var destPtr = mapDest.Scan0;
                for (int y = 0; y < texture2D.Description.Height; y++)
                {
                    // Copy a single line 
                    SharpDX.Utilities.CopyMemory(destPtr, sourcePtr, texture2D.Description.Width * 4);

                    // Advance pointers
                    sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
                    destPtr = IntPtr.Add(destPtr, mapDest.Stride);
                }

                // Release source and dest locks
                currentFrame.UnlockBits(mapDest);
                device.ImmediateContext.UnmapSubresource(texture2D, 0);

                screenResource.Dispose();
                duplicatedOutput.ReleaseFrame();

                return (GetDirectXFrameResult.Success, currentFrame);
            }
            catch (SharpDXException e)
            {
                if (e.ResultCode.Code != SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
                {
                    Logger.Write(e);
                    NeedsInit = true;
                    return (GetDirectXFrameResult.Failure, null);
                }
                return (GetDirectXFrameResult.Timeout, null);
            }
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

                using (var factory = new Factory1())
                {
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
                                        texture2D));
                            }
                            catch (Exception ex)
                            {
                                Logger.Write(ex);
                            }
                        }
                    }
                }


                NeedsInit = false;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private void RefreshCurrentScreenBounds()
        {
            CurrentScreenBounds = Screen.AllScreens[_bitBltScreens[SelectedScreen]].Bounds;
            CaptureFullscreen = true;
            NeedsInit = true;
            ScreenChanged?.Invoke(this, CurrentScreenBounds);
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            RefreshCurrentScreenBounds();
        }
    }
}
