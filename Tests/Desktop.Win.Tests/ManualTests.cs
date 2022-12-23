#nullable disable
using Immense.RemoteControl.Desktop.Shared.Abstractions;
using Immense.RemoteControl.Desktop.Shared.Services;
using Immense.RemoteControl.Desktop.Windows.Services;
using Immense.RemoteControl.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Remotely.Desktop.Win.Services;
using Remotely.Shared.Models;
using Remotely.Shared.Models.RemoteControlDtos;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Remotely.Tests
{
    [TestClass]
    [Ignore("Manual test.")]
    public class ManualTests
    {
        private const int RoundtripLatency = 25;

        private readonly ImageHelper _imageUtils = new(Mock.Of<ILogger<ImageHelper>>());
        private double _bytesSent;
        private double _frameCount;
        private Mock<IAudioCapturer> _audio;
        private ScreenCapturerWin _capturer;
        private Mock<IDesktopHubConnection> _casterSocket;
        private Mock<IClipboardService> _clipboard;
        private AppState _appState;
        private Mock<ICursorIconWatcher> _cursorWatcher;
        private ScreenCaster _screenCaster;
        private Mock<ISessionIndicator> _sessionIndicator;
        private Mock<IShutdownService> _shutdown;
        private Viewer _viewer;


        [TestMethod]
        public async Task CaptureTest()
        {
            var request = new ScreenCastRequest()
            {
                NotifyUser = true,
                RequesterName = "Batman",
                ViewerID = "asdf"
            };

            _screenCaster.BeginScreenCasting(request);

            var timeout = 5_000;

            await Task.Delay(timeout);

            _viewer.DisconnectRequested = true;
            var fps = _frameCount / (timeout / 1000);
            Debug.WriteLine($"FPS: {fps}");
            Debug.WriteLine($"KB Sent: {Math.Round(_bytesSent / 1024, 2)}");
            Assert.IsTrue(fps > 0);
            _viewer.Dispose();
        }


        [TestMethod]
        public void EncodingTests()
        {
            for (var i = 0; i < 2; i++)
            {
                using var frame1 = GetImage("Image1");
                using var frame2 = GetImage("Image1");
                byte[] imageBytes;

                var sw = Stopwatch.StartNew();


                sw.Restart();
                var diff = _imageUtils.GetDiffArea(frame1, frame2, false);

                var diffSize = 0;

                using (var tempImage = _imageUtils.CropBitmap(frame1, diff))
                {
                    imageBytes = _imageUtils.EncodeBitmap(tempImage, SKEncodedImageFormat.Jpeg, 60);
                    diffSize = imageBytes.Length;
                }
                Debug.WriteLine($"Diff area size: {diffSize}");
                Debug.WriteLine($"Diff area time: {sw.Elapsed.TotalMilliseconds}");


                sw.Restart();
                var diffImage = _imageUtils.GetImageDiff(frame1, frame2, false);


                using (var ms = new MemoryStream())
                {
                    diffImage.Value.Encode(ms, SKEncodedImageFormat.Png, 80);
                    Debug.WriteLine($"Diff image size: {ms.ToArray().Length}");
                }
                Debug.WriteLine($"Diff Image time: {sw.Elapsed.TotalMilliseconds}");



                Debug.WriteLine($"\n");
            }
        }


        [TestMethod]
        public void CaptureAndEncodeSpeedTest()
        {
            var iterations = 30;
            var quality = 80;
            var sw = Stopwatch.StartNew();

            SKBitmap currentFrame = new();
            SKBitmap previousFrame = new();

            for (var i = 0; i < iterations; i++)
            {
                previousFrame?.Dispose();
                previousFrame = currentFrame.Copy();
                currentFrame.Dispose();
                
                currentFrame = _capturer.GetNextFrame().Value;
                var diffArea = _imageUtils.GetDiffArea(currentFrame, previousFrame);
                using var cropped = _imageUtils.CropBitmap(currentFrame, diffArea);
                using var skData = cropped.Encode(SKEncodedImageFormat.Webp, quality);
            }
            sw.Stop();
            Console.WriteLine($"GetNextFrame & WEBP: {GetAverage(sw, iterations)}ms per iteration");

            sw.Restart();
            for (var i = 0; i < iterations; i++)
            {
                previousFrame.Dispose();
                previousFrame = currentFrame.Copy();
                currentFrame.Dispose();

                currentFrame = _capturer.GetNextFrame().Value;
                var diffArea = _imageUtils.GetDiffArea(currentFrame, previousFrame);
                using var cropped = _imageUtils.CropBitmap(currentFrame, diffArea);
                using var skData = cropped.Encode(SKEncodedImageFormat.Jpeg, quality);
            }
            sw.Stop();
            Console.WriteLine($"GetNextFrame & JPEG: {GetAverage(sw, iterations)}ms per iteration");
        }


        [TestMethod]
        public void DiffSpeedTests()
        {
            using var bitmap1 = GetImage("Image1");
            using var bitmap2 = GetImage("Image2");
            var iterations = 60;


            var sw = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++)
            {
                _ = ImageUtils.GetDiffArea(bitmap1, bitmap2);
            }
            sw.Stop();
            Console.WriteLine($"Diff Area: {GetAverage(sw, iterations)}ms per call");


            sw.Restart();
            for (var i = 0; i < iterations; i++)
            {
                using var imageDiff = ImageUtils.GetImageDiff(bitmap1, bitmap2).Value;
            }
            sw.Stop();
            Console.WriteLine($"Image Diff: {GetAverage(sw, iterations)}ms per call");
        }

        [TestMethod]
        public void EncodeSpeedTest()
        {
            using var skBitmap = GetImage("Image1");
            var quality = 75;
            var iterations = 30;

            {
                using var skData = skBitmap.Encode(SKEncodedImageFormat.Jpeg, quality);
                Console.WriteLine($"JPEG size: {skData.Size:N0}");
            }

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++)
            {
                using var skData = skBitmap.Encode(SKEncodedImageFormat.Jpeg, quality);
            }
            sw.Stop();
            Console.WriteLine($"JPEG: {GetAverage(sw, iterations)}ms per encode");


            {
                using var skData = skBitmap.Encode(SKEncodedImageFormat.Png, quality);
                Console.WriteLine($"PNG size: {skData.Size:N0}");
            }
            sw.Restart();
            for (var i = 0; i < iterations; i++)
            {
                using var skData = skBitmap.Encode(SKEncodedImageFormat.Png, quality);
            }
            sw.Stop();
            Console.WriteLine($"PNG: {GetAverage(sw, iterations)}ms per encode");


            {
                using var skData = skBitmap.Encode(SKEncodedImageFormat.Webp, quality);
                Console.WriteLine($"WEBP size: {skData.Size:N0}");
            }
            sw.Restart();
            for (var i = 0; i < iterations; i++)
            {
                using var skData = skBitmap.Encode(SKEncodedImageFormat.Webp, quality);
            }
            sw.Stop();
            Console.WriteLine($"WEBP: {GetAverage(sw, iterations)}ms per encode");
        }

        [TestMethod]
        public void GetDiffAreaTest()
        {
            using var bitmap1 = GetImage("Image1");
            using var bitmap2 = GetImage("Image2");

            var diffArea = ImageUtils.GetDiffArea(bitmap1, bitmap2);
            using var cropped = ImageUtils.CropBitmap(bitmap2, diffArea);

            SaveFile(cropped, "Test.webp");
        }

        [TestMethod]
        public void GetImageDiffTest()
        {
            using var bitmap1 = GetImage("Image1");
            using var bitmap2 = GetImage("Image2");

            var diff = ImageUtils.GetImageDiff(bitmap1, bitmap2);

            SaveFile(diff.Value, "Test.webp");
        }

        [TestInitialize]
        public void Init()
        {
            _appState = new Conductor();
            _cursorWatcher = new Mock<ICursorIconWatcher>();
            _sessionIndicator = new Mock<ISessionIndicator>();
            _clipboard = new Mock<IClipboardService>();
            _casterSocket = new Mock<ICasterSocket>();
            _audio = new Mock<IAudioCapturer>();
            _shutdown = new Mock<IShutdownService>();
            _capturer = new ScreenCapturerWin();
            _screenCaster = new ScreenCaster(_appState, _cursorWatcher.Object, _sessionIndicator.Object, _shutdown.Object);
            _viewer = new Viewer(_casterSocket.Object, _capturer, _clipboard.Object, _audio.Object);

            _casterSocket
                .Setup(x => x.SendDtoToViewer(It.IsAny<CaptureFrameDto>(), It.IsAny<string>()))
                .Callback((CaptureFrameDto dto, string viewerId) =>
                {
                    _bytesSent += dto.ImageBytes.Length;
                });

            _casterSocket
                .Setup(x => x.SendDtoToViewer(It.Is<CaptureFrameDto>(x => x.EndOfFrame), It.IsAny<string>()))
                .Callback(() =>
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(RoundtripLatency);
                        _viewer.PendingSentFrames.TryDequeue(out _);
                    });
                });

            _casterSocket
                .Setup(x => x.IsConnected)
                .Returns(true)
                .Callback(() => _frameCount++);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConsole().AddDebug().AddEventLog();
            });

            serviceCollection.AddSingleton((e) => _viewer);

            ServiceContainer.Instance = serviceCollection.BuildServiceProvider();
        }


        private static double GetAverage(Stopwatch sw, int iterations)
        {
            return Math.Round(sw.Elapsed.TotalMilliseconds / iterations, 2);
        }

        private SKBitmap GetImage(string imageFileName)
        {
            using var mrs = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Remotely.Desktop.Win.Tests.Resources.{imageFileName}.jpg");
            var resourceImage = (Bitmap)Bitmap.FromStream(mrs);

            if (resourceImage.PixelFormat != PixelFormat.Format32bppArgb)
            {
                return resourceImage
                    .Clone(new Rectangle(0, 0, resourceImage.Width, resourceImage.Height), PixelFormat.Format32bppArgb)
                    .ToSKBitmap();
            }
            return resourceImage.ToSKBitmap();
        }

        private static void SaveFile(
          SKBitmap bitmap,
          string fileName,
          SKEncodedImageFormat format = SKEncodedImageFormat.Webp,
          int quality = 80)
        {
            var savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            using var fs = new FileStream(savePath, FileMode.Create);
            bitmap.Encode(fs, format, quality);
        }
    }
}
