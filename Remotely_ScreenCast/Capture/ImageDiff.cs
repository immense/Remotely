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
        private static EncoderParameters EncoderParams { get; } = new EncoderParameters()
        {
            Param = new EncoderParameter[] { new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 0) }
        };

        private static ImageCodecInfo CodecInfo { get; } = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == ImageFormat.Png.Guid);

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

            var mergedFrame = new Bitmap(width, height);

            var bd1 = previousFrame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, currentFrame.PixelFormat);
            var bd2 = currentFrame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, previousFrame.PixelFormat);
			var bd3 = mergedFrame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, currentFrame.PixelFormat);


			// Get the address of the first line.
			IntPtr ptr1 = bd1.Scan0;
            IntPtr ptr2 = bd2.Scan0;
			IntPtr ptr3 = bd3.Scan0;

			// Declare an array to hold the bytes of the bitmap.
			int arraySize = Math.Abs(bd1.Stride) * currentFrame.Height;
            var rgbValues1 = new byte[arraySize];
            var rgbValues2 = new byte[arraySize];
			var rgbValues3 = new byte[arraySize];

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
                // Byte array that indicates top left coordinates of the image.
                bitmap.Save(ms, CodecInfo, EncoderParams);
                // Byte array that indicates top left coordinates of the image.
                return ms.ToArray();
            }
        }
	}
}
