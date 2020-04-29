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
using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Services;
using Remotely.ScreenCast.Win.Models;
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
using System.Windows.Forms;

namespace Remotely.ScreenCast.Win.Services
{
    public class ScreenCapturerWin : IScreenCapturer
    {
        private readonly Dictionary<string, int> bitBltScreens = new Dictionary<string, int>();
        private readonly Dictionary<string, DirectXOutput> directxScreens = new Dictionary<string, DirectXOutput>();
        public ScreenCapturerWin()
        {
            Init();
            GetBitBltFrame();
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        public event EventHandler<Rectangle> ScreenChanged;

        public bool CaptureFullscreen { get; set; } = true;
        public Bitmap CurrentFrame { get; set; }
        public Rectangle CurrentScreenBounds { get; private set; } = Screen.PrimaryScreen.Bounds;
        public bool NeedsInit { get; set; } = true;
        public Bitmap PreviousFrame { get; set; }
        public string SelectedScreen { get; private set; } = Screen.PrimaryScreen.DeviceName;
        private Graphics Graphic { get; set; }

        public void Dispose()
        {
            SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
            ClearDirectXOutputs();

            CurrentFrame?.Dispose();
            PreviousFrame?.Dispose();
        }
        public IEnumerable<string> GetDisplayNames() => Screen.AllScreens.Select(x => x.DeviceName);

        public void GetNextFrame()
        {
            try
            {
                if (NeedsInit)
                {
                    Logger.Write("Init needed in DXCapture.  Switching desktops.");
                    if (Win32Interop.SwitchToInputDesktop())
                    {
                        Win32Interop.GetCurrentDesktop(out var desktopName);
                        Logger.Write($"Switch to desktop {desktopName} after capture error in DXCapture.");
                    }
                    Init();
                }

                Win32Interop.SwitchToInputDesktop();

                PreviousFrame?.Dispose();
                PreviousFrame = (Bitmap)CurrentFrame.Clone();

                if (directxScreens.ContainsKey(SelectedScreen))
                {
                    GetDirectXFrame();
                }
                else
                {
                    GetBitBltFrame();
                }
            }
            catch (Exception e)
            {
                Logger.Write(e);
                NeedsInit = true;
            }
        }

        public int GetScreenCount() => Screen.AllScreens.Length;

        public int GetSelectedScreenIndex() => bitBltScreens[SelectedScreen];

        public Rectangle GetVirtualScreenBounds() => SystemInformation.VirtualScreen;

        public void Init()
        {
            CurrentFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);
            PreviousFrame = new Bitmap(CurrentScreenBounds.Width, CurrentScreenBounds.Height, PixelFormat.Format32bppArgb);

            InitBitBlt();
            InitDirectX();

            ScreenChanged?.Invoke(this, CurrentScreenBounds);
        }

        public void SetSelectedScreen(string displayName)
        {
            if (displayName == SelectedScreen)
            {
                return;
            }

            if (bitBltScreens.ContainsKey(displayName))
            {
                SelectedScreen = displayName;
            }
            else
            {
                SelectedScreen = bitBltScreens.Keys.First();
            }
            RefreshCurrentScreenBounds();
        }

        private void ClearDirectXOutputs()
        {
            foreach (var screen in directxScreens.Values)
            {
                screen.Dispose();
            }
            directxScreens.Clear();
        }

        private void GetBitBltFrame()
        {
            try
            {
                Win32Interop.SwitchToInputDesktop();
                PreviousFrame.Dispose();
                PreviousFrame = (Bitmap)CurrentFrame.Clone();
                Graphic.CopyFromScreen(CurrentScreenBounds.Left, CurrentScreenBounds.Top, 0, 0, new Size(CurrentScreenBounds.Width, CurrentScreenBounds.Height));
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Logger.Write("Capturer error.  Trying to switch desktops in BitBltCapture.");
                if (Win32Interop.SwitchToInputDesktop())
                {
                    Win32Interop.GetCurrentDesktop(out var desktopName);
                    Logger.Write($"Switch to desktop {desktopName} after capture error in BitBltCapture.");
                }
                NeedsInit = true;
            }
        }

        private void GetDirectXFrame()
        {
            try
            {
                var duplicatedOutput = directxScreens[SelectedScreen].OutputDuplication;
                var device = directxScreens[SelectedScreen].Device;
                var texture2D = directxScreens[SelectedScreen].Texture2D;

                // Try to get duplicated frame within given time is ms
                var result = duplicatedOutput.TryAcquireNextFrame(100,
                    out var duplicateFrameInformation,
                    out var screenResource);

                if (result.Failure)
                {
                    if (result.Code == SharpDX.DXGI.ResultCode.WaitTimeout.Code)
                    {
                        return;
                    }
                    else
                    {
                        Logger.Write($"TryAcquireFrame error.  Code: {result.Code}");
                        NeedsInit = true;
                        return;
                    }
                }

                if (duplicateFrameInformation.AccumulatedFrames == 0)
                {
                    try
                    {
                        duplicatedOutput.ReleaseFrame();
                    }
                    catch { }
                    return;
                }

                // TODO: Get dirty rects.
                //RawRectangle[] dirtyRectsBuffer = new RawRectangle[duplicateFrameInformation.TotalMetadataBufferSize];
                //duplicatedOutput.GetFrameDirtyRects(duplicateFrameInformation.TotalMetadataBufferSize, dirtyRectsBuffer, out var dirtyRectsSizeRequired);

                // copy resource into memory that can be accessed by the CPU
                using (var screenTexture2D = screenResource.QueryInterface<Texture2D>())
                {
                    device.ImmediateContext.CopyResource(screenTexture2D, texture2D);
                }

                // Get the desktop capture texture
                var mapSource = device.ImmediateContext.MapSubresource(texture2D, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

                var boundsRect = new Rectangle(0, 0, texture2D.Description.Width, texture2D.Description.Height);

                // Copy pixels from screen capture Texture to GDI bitmap
                var mapDest = CurrentFrame.LockBits(boundsRect, ImageLockMode.WriteOnly, CurrentFrame.PixelFormat);
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
                CurrentFrame.UnlockBits(mapDest);
                device.ImmediateContext.UnmapSubresource(texture2D, 0);

                screenResource.Dispose();
                duplicatedOutput.ReleaseFrame();
            }
            catch (SharpDXException e)
            {
                if (e.ResultCode.Code != SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
                {
                    Logger.Write(e);
                    NeedsInit = true;
                }
            }
        }

        private void InitBitBlt()
        {
            bitBltScreens.Clear();
            Graphic = Graphics.FromImage(CurrentFrame);
            for (var i = 0; i < Screen.AllScreens.Length; i++)
            {
                bitBltScreens.Add(Screen.AllScreens[i].DeviceName, i);
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
                        try
                        {
                            var device = new SharpDX.Direct3D11.Device(adapter);
                            var output = adapter.Outputs.FirstOrDefault();
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

                            directxScreens.Add(
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


                NeedsInit = false;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private void RefreshCurrentScreenBounds()
        {
            CurrentScreenBounds = Screen.AllScreens[bitBltScreens[SelectedScreen]].Bounds;
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
