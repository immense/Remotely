using Remotely_ScreenCapture.Utilities;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Win32;

namespace Remotely_ScreenCast.Capture
{
    public class DXCapture : ICapturer
    {
        public Bitmap PreviousFrame { get; set; }
        public Bitmap CurrentFrame { get; set; }
        public Rectangle CurrentScreenBounds { get; private set; }
        public bool CaptureFullscreen { get; set; } = true;
        public EventHandler<Rectangle> ScreenChanged { get; set; }
        public int SelectedScreen
        {
            get
            {
                return selectedScreen;
            }
            set
            {
                if (value == selectedScreen)
                {
                    return;
                }
                if (adapter == null)
                {
                    selectedScreen = 0;
                }
                else
                {
                    if (adapter.Outputs.Length >= value + 1)
                    {
                        selectedScreen = value;
                    }
                    else
                    {
                        selectedScreen = 0;
                    }
                }
                NeedsInit = true;
            }
        }
        public bool NeedsInit { get; set; } = true;

        private string desktopName;
		private Factory1 factory;
		private Adapter1 adapter;
		private SharpDX.Direct3D11.Device device;
		private Output output;
		private Output1 output1;
		private int width;
		private int height;
		private Texture2DDescription textureDesc;
		private Texture2D screenTexture;
		private OutputDuplication duplicatedOutput;
        private int selectedScreen = 0;

        private void Init()
		{
			desktopName = Win32Interop.GetCurrentDesktop();

			factory = new Factory1();

			//Get first adapter
			adapter = factory.Adapters1.FirstOrDefault(x => x.Outputs.Length > 0);
			//Get device from adapter
			device = new SharpDX.Direct3D11.Device(adapter);
			//Get front buffer of the adapter
            if (adapter.GetOutputCount() < selectedScreen + 1)
            {
                selectedScreen = 0;
            }
			output = adapter.GetOutput(selectedScreen);
			output1 = output.QueryInterface<Output1>();

            // Width/Height of desktop to capture
            var bounds = output.Description.DesktopBounds;
			var newWidth = bounds.Right - bounds.Left;
			var newHeight = bounds.Bottom - bounds.Top;
            CurrentScreenBounds = new Rectangle(bounds.Left, bounds.Top, newWidth, newHeight);
            if (newWidth != width || newHeight != height)
            {
                ScreenChanged?.Invoke(this, CurrentScreenBounds);
            }
            width = newWidth;
            height = newHeight;

            CurrentFrame = new Bitmap(width, height);
            PreviousFrame = new Bitmap(width, height);

            // Create Staging texture CPU-accessible
            textureDesc = new Texture2DDescription
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
			screenTexture = new Texture2D(device, textureDesc);
			duplicatedOutput = output1.DuplicateOutput(device);

		}
		public void Capture()
		{
			try
			{
                if (NeedsInit)
                {
                    Init();
                    NeedsInit = false;
                }
				var currentDesktop = Win32Interop.GetCurrentDesktop();
				Console.WriteLine($"Using DXCapture.");
				Console.WriteLine($"Current Desktop: {currentDesktop}");
				if (currentDesktop != desktopName)
				{
					desktopName = currentDesktop;
					var inputDesktop = Win32Interop.OpenInputDesktop();
					var success = User32.SetThreadDesktop(inputDesktop);
					User32.CloseDesktop(inputDesktop);
					Logger.Write($"Set thread desktop: {success}");
                    return;
				}

				SharpDX.DXGI.Resource screenResource;
				OutputDuplicateFrameInformation duplicateFrameInformation;

				// Try to get duplicated frame within given time is ms
				duplicatedOutput.AcquireNextFrame(50, out duplicateFrameInformation, out screenResource);

				while (duplicateFrameInformation.AccumulatedFrames < 1)
				{
					duplicatedOutput.ReleaseFrame();
					duplicatedOutput.AcquireNextFrame(50, out duplicateFrameInformation, out screenResource);
				}

				// copy resource into memory that can be accessed by the CPU
				using (var screenTexture2D = screenResource.QueryInterface<Texture2D>())
					device.ImmediateContext.CopyResource(screenTexture2D, screenTexture);

				// Get the desktop capture texture
				var mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

				// Create Drawing.Bitmap
				using (var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
				{
					var boundsRect = new Rectangle(0, 0, width, height);

					// Copy pixels from screen capture Texture to GDI bitmap
					var mapDest = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
					var sourcePtr = mapSource.DataPointer;
					var destPtr = mapDest.Scan0;
					for (int y = 0; y < height; y++)
					{
						// Copy a single line 
						Utilities.CopyMemory(destPtr, sourcePtr, width * 4);

						// Advance pointers
						sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
						destPtr = IntPtr.Add(destPtr, mapDest.Stride);
					}

					// Release source and dest locks
					bitmap.UnlockBits(mapDest);
					device.ImmediateContext.UnmapSubresource(screenTexture, 0);

					PreviousFrame = (Bitmap)CurrentFrame.Clone();
					CurrentFrame = (Bitmap)bitmap.Clone();

                    screenResource.Dispose();
                    duplicatedOutput.ReleaseFrame();
				}
			}
			catch (SharpDXException e)
			{
				if (e.ResultCode.Code != SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
				{
                    Logger.Write(e);
					Init();
					Capture();
				}
			}
			catch (Exception e)
			{
                Logger.Write(e);
				Init();
				Capture();
			}
		}

        public Point GetAbsoluteScreenCoordinatesFromPercentages(decimal percentX, decimal percentY)
        {
            var absoluteX = (CurrentScreenBounds.Width * percentX) + CurrentScreenBounds.Left;
            var absoluteY = (CurrentScreenBounds.Height * percentY) + CurrentScreenBounds.Top;
            return new Point((int)absoluteX, (int)absoluteY);
        }
    }
}
