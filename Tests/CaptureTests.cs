using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotely.ScreenCast.Core.Utilities;
using Remotely.ScreenCast.Win.Services;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Remotely.Tests
{
    [TestClass]
    public class CaptureTests
    {
        [TestMethod]
        [Ignore("Debug only.")]
        public async Task Capture()
        {
            var capturer = new ScreenCapturerWin();
            capturer.GetNextFrame();
            Process.Start("msg", "* test");
            await Task.Delay(500);
            capturer.GetNextFrame();
            var diffArea = ImageUtils.GetDiffArea(capturer.CurrentFrame, capturer.PreviousFrame, false);
            if (diffArea.IsEmpty)
            {
                return;
            }
            using (var newImage = capturer.CurrentFrame.Clone(diffArea, PixelFormat.Format32bppArgb))
            {
                if (capturer.CaptureFullscreen)
                {
                    capturer.CaptureFullscreen = false;
                }

                newImage.Save(Path.Combine(Path.GetTempPath(), "!ImageDiff.jpg"), ImageFormat.Jpeg);
                capturer.CurrentFrame.Save(Path.Combine(Path.GetTempPath(), "!Current.jpg"), ImageFormat.Jpeg);
                capturer.PreviousFrame.Save(Path.Combine(Path.GetTempPath(), "!Previous.jpg"), ImageFormat.Jpeg);
            }
        }


        private double MeasureCommand(Action command)
        {
            var stopwatch = Stopwatch.StartNew();
            command.Invoke();
            stopwatch.Stop();
            Console.WriteLine($"Command time: {stopwatch.Elapsed.TotalMilliseconds}");
            return stopwatch.Elapsed.TotalMilliseconds;
        }
    }
}
