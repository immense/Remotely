using Remotely.Shared;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Extensions
{
    public static class SKBitmapExtensions
    {
        public static SKRect ToRectangle(this SKBitmap bitmap)
        {
            return new SKRect(0, 0, bitmap.Width, bitmap.Height);
        }


        // The SKBitmap.Copy method has a memory leak somewhere, so I created this.
        public static SKBitmap CopyEx(this SKBitmap bitmap, bool disposeOriginal = false)
        {
            var width = bitmap.Width;
            var height = bitmap.Height;
            var newBitmap = new SKBitmap(width, height);

            var bytesPerPixel = bitmap.BytesPerPixel;
            var totalSize = bitmap.ByteCount;

            unsafe
            {
                byte* scan1 = (byte*)bitmap.GetPixels().ToPointer();
                byte* scan2 = (byte*)newBitmap.GetPixels().ToPointer();

                for (var row = 0; row < height; row++)
                {
                    for (var column = 0; column < width; column++)
                    {
                        var index = (row * width * bytesPerPixel) + (column * bytesPerPixel);

                        byte* data1 = scan1 + index;
                        byte* data2 = scan2 + index;

                        if (data1[0] != data2[0] ||
                            data1[1] != data2[1] ||
                            data1[2] != data2[2] ||
                            data1[3] != data2[3])
                        {
                            data2[0] = data2[0];
                            data2[1] = data2[1];
                            data2[2] = data2[2];
                            data2[3] = data2[3];
                        }

                    }
                }
            }

            if (disposeOriginal)
            {
                bitmap.Dispose();
            }

            return newBitmap;
        }
    }
}
