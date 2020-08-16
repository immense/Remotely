using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Remotely.Desktop.Core.Utilities
{
    public class ImageUtils
    {
        public static ImageCodecInfo JpegEncoder { get; } = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == ImageFormat.Jpeg.Guid);
        public static byte[] EncodeBitmap(Bitmap bitmap, EncoderParameters encoderParams)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, JpegEncoder, encoderParams);
                return ms.ToArray();
            }
        }

        public static Rectangle GetDiffArea(Bitmap currentFrame, Bitmap previousFrame, bool captureFullscreen)
        {
            if (currentFrame == null || previousFrame == null)
            {
                return Rectangle.Empty;
            }

            if (captureFullscreen)
            {
                return new Rectangle(new Point(0, 0), currentFrame.Size);
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
            int left = int.MaxValue;
            int top = int.MaxValue;
            int right = int.MinValue;
            int bottom = int.MinValue;

            BitmapData bd1 = null;
            BitmapData bd2 = null;

            try
            {
                bd1 = previousFrame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, currentFrame.PixelFormat);
                bd2 = currentFrame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, previousFrame.PixelFormat);

                var bytesPerPixel = Bitmap.GetPixelFormatSize(currentFrame.PixelFormat) / 8;
                var totalSize = bd1.Height * bd1.Width * bytesPerPixel;

                unsafe
                {
                    byte* scan1 = (byte*)bd1.Scan0.ToPointer();
                    byte* scan2 = (byte*)bd2.Scan0.ToPointer();

                    for (int counter = 0; counter < totalSize - bytesPerPixel; counter += bytesPerPixel)
                    {
                        byte* data1 = scan1 + counter;
                        byte* data2 = scan2 + counter;

                        if (data1[0] != data2[0] ||
                            data1[1] != data2[1] ||
                            data1[2] != data2[2] ||
                            data1[3] != data2[3])
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
                }

                if (left <= right && top <= bottom)
                {
                    // Bounding box is valid.  Padding is necessary to prevent artifacts from
                    // moving windows.
                    left = Math.Max(left - 10, 0);
                    top = Math.Max(top - 10, 0);
                    right = Math.Min(right + 10, width);
                    bottom = Math.Min(bottom + 10, height);

                    return new Rectangle(left, top, right - left, bottom - top);
                }
                else
                {
                    return Rectangle.Empty;
                }
            }
            catch
            {
                return Rectangle.Empty;
            }
            finally
            {
                currentFrame.UnlockBits(bd1);
                previousFrame.UnlockBits(bd2);
            }
        }

        public static Bitmap GetImageDiff(Bitmap currentFrame, Bitmap previousFrame, bool captureFullscreen)
        {
            if (captureFullscreen)
            {
                return (Bitmap)currentFrame.Clone();
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
    }
}
