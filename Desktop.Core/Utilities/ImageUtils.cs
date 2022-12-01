using Microsoft.IO;
using Remotely.Desktop.Core.Extensions;
using Remotely.Shared;
using Remotely.Shared.Utilities;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Desktop.Core.Utilities
{
    public class ImageUtils
    {
        private static readonly RecyclableMemoryStreamManager _recycleManager = new();

        public static byte[] EncodeBitmap(SKBitmap bitmap, SKEncodedImageFormat format, int quality)
        {
            using var ms = _recycleManager.GetStream();
            bitmap.Encode(ms, format, quality);
            return ms.ToArray();
        }

        public static SKBitmap CropBitmap(SKBitmap bitmap, SKRect cropArea)
        {
            var cropped = new SKBitmap((int)cropArea.Width, (int)cropArea.Height);
            using var canvas = new SKCanvas(cropped);
            canvas.DrawBitmap(
                bitmap,
                cropArea,
                new SKRect(0, 0, cropArea.Width, cropArea.Height));
            return cropped;
        }

        public static Result<SKBitmap> GetImageDiff(SKBitmap currentFrame, SKBitmap previousFrame, bool forceFullscreen = false)
        {
            try
            {
                if (currentFrame is null)
                {
                    return Result.Fail<SKBitmap>("Current frame cannot be null.");
                }

                if (previousFrame is null || forceFullscreen)
                {
                    return Result.Ok(currentFrame.Copy());
                }


                if (currentFrame.Height != previousFrame.Height ||
                    currentFrame.Width != previousFrame.Width ||
                    currentFrame.BytesPerPixel != previousFrame.BytesPerPixel)
                {
                    return Result.Fail<SKBitmap>("Frames are not of equal size.");
                }

                var width = currentFrame.Width;
                var height = currentFrame.Height;
                var anyChanges = false;
                var diffFrame = new SKBitmap(width, height);

                var bytesPerPixel = currentFrame.BytesPerPixel;
                var totalSize = currentFrame.ByteCount;

                unsafe
                {
                    byte* scan1 = (byte*)currentFrame.GetPixels().ToPointer();
                    byte* scan2 = (byte*)previousFrame.GetPixels().ToPointer();
                    byte* scan3 = (byte*)diffFrame.GetPixels().ToPointer();

                    for (var row = 0; row < height; row++)
                    {
                        for (var column = 0; column < width; column++)
                        {
                            var index = (row * width * bytesPerPixel) + (column * bytesPerPixel);

                            byte* data1 = scan1 + index;
                            byte* data2 = scan2 + index;
                            byte* data3 = scan3 + index;

                            if (data1[0] != data2[0] ||
                                data1[1] != data2[1] ||
                                data1[2] != data2[2] ||
                                data1[3] != data2[3])
                            {
                                anyChanges = true;
                                data3[0] = data2[0];
                                data3[1] = data2[1];
                                data3[2] = data2[2];
                                data3[3] = data2[3];
                            }

                        }
                    }
                }

                if (anyChanges)
                {
                    return Result.Ok(diffFrame);
                }

                diffFrame.Dispose();
                return Result.Fail<SKBitmap>("No difference found.");
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error while getting image diff.");
                return Result.Fail<SKBitmap>(ex);
            }
        }
        public static SKRect GetDiffArea(SKBitmap currentFrame, SKBitmap previousFrame, bool forceFullscreen = false)
        {
            try
            {
                if (currentFrame is null)
                {
                    return SKRect.Empty;
                }

                if (previousFrame is null || forceFullscreen)
                {
                    return currentFrame.ToRectangle();
                }


                if (currentFrame.Height != previousFrame.Height ||
                    currentFrame.Width != previousFrame.Width ||
                    currentFrame.BytesPerPixel != previousFrame.BytesPerPixel)
                {
                    return SKRect.Empty;
                }

                var width = currentFrame.Width;
                var height = currentFrame.Height;
                int left = int.MaxValue;
                int top = int.MaxValue;
                int right = int.MinValue;
                int bottom = int.MinValue;

                var bytesPerPixel = currentFrame.BytesPerPixel;
                var totalSize = currentFrame.ByteCount;

                unsafe
                {
                    byte* scan1 = (byte*)currentFrame.GetPixels().ToPointer();
                    byte* scan2 = (byte*)previousFrame.GetPixels().ToPointer();

                    for (var row = 0; row < height; row++)
                    {
                        for (var column = 0; column < width; column++)
                        {
                            var index = (row * width * bytesPerPixel) + (column * bytesPerPixel);

                            byte* data1 = scan1 + index;
                            byte* data2 = scan2 + index;

                            if (data1[0] != data2[0] ||
                                data1[1] != data2[1] ||
                                data1[2] != data2[2])
                            {

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

                    // Check for valid bounding box.
                    if (left <= right && top <= bottom)
                    {
                        left = Math.Max(left - 2, 0);
                        top = Math.Max(top - 2, 0);
                        right = Math.Min(right + 2, width);
                        bottom = Math.Min(bottom + 2, height);
                        return new SKRect(left, top, right, bottom);
                    }

                    return SKRect.Empty;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Error while getting area diff.");
                return SKRect.Empty;
            }
        }
    }
}
