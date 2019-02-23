using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Remotely_ScreenCast.Capture
{
    public class ImageDiff
    {

        private static BitmapData bd1;
        private static BitmapData bd2;
        private static BitmapData bd3;
        private static Bitmap mergedFrame;
        private static byte[] rgbValues1;
        private static byte[] rgbValues2;
        private static byte[] rgbValues3;

        public static Bitmap GetImageDiff(Bitmap currentFrame, Bitmap previousFrame, bool captureFullscreen)
        {
            if (currentFrame.Height != previousFrame.Height || currentFrame.Width != previousFrame.Width)
            {
                throw new Exception("Bitmaps are not of equal dimensions.");
            }
            if (!Bitmap.IsAlphaPixelFormat(currentFrame.PixelFormat) || !Bitmap.IsAlphaPixelFormat(previousFrame.PixelFormat) ||
                !Bitmap.IsCanonicalPixelFormat(currentFrame.PixelFormat) || !Bitmap.IsCanonicalPixelFormat(previousFrame.PixelFormat))
            {
                throw new Exception("Bitmaps must be 32 bits per pixel and contain alpha channel.");
            }
            var width = currentFrame.Width;
            var height = currentFrame.Height;

            mergedFrame = new Bitmap(width, height);

            bd1 = previousFrame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, currentFrame.PixelFormat);
            bd2 = currentFrame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, previousFrame.PixelFormat);
			bd3 = mergedFrame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, currentFrame.PixelFormat);


			// Get the address of the first line.
			IntPtr ptr1 = bd1.Scan0;
            IntPtr ptr2 = bd2.Scan0;
			IntPtr ptr3 = bd3.Scan0;

			// Declare an array to hold the bytes of the bitmap.
			int arraySize = Math.Abs(bd1.Stride) * currentFrame.Height;
            rgbValues1 = new byte[arraySize];
            rgbValues2 = new byte[arraySize];
			rgbValues3 = new byte[arraySize];

			// Copy the RGBA values into the array.
			Marshal.Copy(ptr1, rgbValues1, 0, arraySize);
            Marshal.Copy(ptr2, rgbValues2, 0, arraySize);

            if (captureFullscreen)
            {
                previousFrame.UnlockBits(bd1);
                currentFrame.UnlockBits(bd2);
				mergedFrame.UnlockBits(bd3);
				return currentFrame;
            }

            // Check RGBA value for each pixel.
            for (int counter = 0; counter < rgbValues2.Length - 4; counter += 4)
            {
                if (rgbValues1[counter] != rgbValues2[counter] ||
                    rgbValues1[counter + 1] != rgbValues2[counter + 1] ||
                    rgbValues1[counter + 2] != rgbValues2[counter + 2] ||
                    rgbValues1[counter + 3] != rgbValues2[counter + 3])
                {
					// Change was found.
					rgbValues3[counter] = rgbValues2[counter];
					rgbValues3[counter + 1] = rgbValues2[counter + 1];
					rgbValues3[counter + 2] = rgbValues2[counter + 2];
					rgbValues3[counter + 3] = rgbValues2[counter + 3];
				}
            }

			// Copy merged frame to bitmap.
			Marshal.Copy(rgbValues3, 0, ptr3, rgbValues3.Length);

			previousFrame.UnlockBits(bd1);
            currentFrame.UnlockBits(bd2);
			mergedFrame.UnlockBits(bd3);

			return mergedFrame;
        }

        public static byte[] EncodeBitmap(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                // Byte array that indicates top left coordinates of the image.
                return ms.ToArray();
            }
        }


		public byte[] GetDiffImage(Rectangle imageArea, Bitmap CurrentFrame)
		{
			using (var ms = new MemoryStream())
			{
				using (var croppedFrame = CurrentFrame.Clone(imageArea, PixelFormat.Format24bppRgb))
				{
					var encoderParams = new EncoderParameters(1);
					encoderParams.Param = new EncoderParameter[] { new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 1) };
					croppedFrame.Save(ms, ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == ImageFormat.Png.Guid), encoderParams);
					// Byte array that indicates top left coordinates of the image.
					byte[] header;
					using (var headerStream = new MemoryStream())
					{
						var formatter = new BinaryFormatter();
						formatter.Serialize(headerStream, new Point(imageArea.X, imageArea.Y));
						header = headerStream.ToArray();
					}

					return header.Concat(ms.ToArray()).ToArray();
				}
			}

		}

		public Rectangle GetDiffArea(Bitmap previousFrame, Bitmap currentFrame, bool captureFullscreen)
		{
			if (captureFullscreen)
			{
				captureFullscreen = false;
				return new Rectangle(0, 0, currentFrame.Width, currentFrame.Height);
			}
			if (currentFrame.Height != previousFrame.Height || currentFrame.Width != previousFrame.Width)
			{
				throw new Exception("Bitmaps are not of equal dimensions.");
			}
			if (!Bitmap.IsAlphaPixelFormat(currentFrame.PixelFormat) || !Bitmap.IsAlphaPixelFormat(previousFrame.PixelFormat) ||
				!Bitmap.IsCanonicalPixelFormat(currentFrame.PixelFormat) || !Bitmap.IsCanonicalPixelFormat(previousFrame.PixelFormat))
			{
				throw new Exception("Bitmaps must be 32 bits per pixel and contain alpha channel.");
			}
			var width = currentFrame.Width;
			var height = currentFrame.Height;
			var left = int.MaxValue;
			var top = int.MaxValue;
			var right = int.MinValue;
			var bottom = int.MinValue;

			try
			{
				bd1 = currentFrame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, currentFrame.PixelFormat);
			}
			catch
			{
				try
				{
					currentFrame.UnlockBits(bd1);
					bd1 = currentFrame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, currentFrame.PixelFormat);
				}
				catch
				{
					return Rectangle.Empty;
				}
			}
			try
			{
				bd2 = previousFrame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, previousFrame.PixelFormat);
			}
			catch
			{
				try
				{
					previousFrame.UnlockBits(bd2);
					bd2 = previousFrame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, previousFrame.PixelFormat);
				}
				catch
				{
					return Rectangle.Empty;
				}
			}
			// Get the address of the first line.
			IntPtr ptr1 = bd1.Scan0;
			IntPtr ptr2 = bd2.Scan0;

			// Declare an array to hold the bytes of the bitmap.
			int arraySize = Math.Abs(bd1.Stride) * currentFrame.Height;
			rgbValues1 = new byte[arraySize];
			rgbValues2 = new byte[arraySize];

			// Copy the RGBA values into the array.
			Marshal.Copy(ptr1, rgbValues1, 0, arraySize);
			Marshal.Copy(ptr2, rgbValues2, 0, arraySize);

			// Check RGBA value for each pixel.
			for (int counter = 0; counter < rgbValues1.Length - 4; counter += 4)
			{
				if (rgbValues1[counter] != rgbValues2[counter] ||
					rgbValues1[counter + 1] != rgbValues2[counter + 1] ||
					rgbValues1[counter + 2] != rgbValues2[counter + 2] ||
					rgbValues1[counter + 3] != rgbValues2[counter + 3])
				{
					// Change was found.
					var pixel = counter / 4;
					var row = (int)Math.Floor((double)pixel / bd1.Width);
					var column = pixel % bd1.Width;
					if (row < top)
					{
						top = row;
					}
					if (row > bottom)
					{
						bottom = row;
					}
					if (column < left)
					{
						left = column;
					}
					if (column > right)
					{
						right = column;
					}
				}
			}

			if (left < right && top < bottom)
			{
				// Bounding box is valid.

				left = Math.Max(left - 20, 0);
				top = Math.Max(top - 20, 0);
				right = Math.Min(right + 20, currentFrame.Width);
				bottom = Math.Min(bottom + 20, currentFrame.Height);

				currentFrame.UnlockBits(bd1);
				previousFrame.UnlockBits(bd2);

				return new Rectangle(left, top, right - left, bottom - top);
			}
			else
			{
				currentFrame.UnlockBits(bd1);
				previousFrame.UnlockBits(bd2);
				return Rectangle.Empty;
			}
		}

	}
}
