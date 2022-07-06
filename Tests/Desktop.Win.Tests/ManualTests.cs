#nullable disable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Core.Utilities;
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
    public class ManualTests
    {
        private const int RoundtripLatency = 25;

        private double _bytesSent;
        private double _frameCount;
        private Mock<IAudioCapturer> _audio;
        private ScreenCapturerWin _capturer;
        private Mock<ICasterSocket> _casterSocket;
        private Mock<IClipboardService> _clipboard;
        private Conductor _conductor;
        private Mock<ICursorIconWatcher> _cursorWatcher;
        private ScreenCaster _screenCaster;
        private Mock<ISessionIndicator> _sessionIndicator;
        private Mock<IShutdownService> _shutdown;
        private Mock<IWebRtcSessionFactory> _webrtcFactory;
        private Viewer _viewer;


        [TestMethod]
        [Ignore("Manual test.")]
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
        [Ignore("Manual test.")]
        public void EncodingTests()
        {
            for (var i = 0; i < 2; i++)
            {
                using var frame1 = GetFrame("Frame1");
                using var frame2 = GetFrame("Frame2");
                var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
                byte[] imageBytes;

                var sw = Stopwatch.StartNew();


                sw.Restart();
                var diff = ImageUtils.GetDiffArea(frame1, frame2, false);

                var diffSize = 0;

                using (var tempImage = ImageUtils.CropBitmap(frame1, diff))
                {
                    imageBytes = ImageUtils.EncodeBitmap(tempImage, SKEncodedImageFormat.Jpeg, 60);
                    diffSize = imageBytes.Length;
                }
                Debug.WriteLine($"Diff area size: {diffSize}");
                Debug.WriteLine($"Diff area time: {sw.Elapsed.TotalMilliseconds}");


                sw.Restart();
                var diffImage = ImageUtils.GetImageDiff(frame1, frame2, false);


                using (var ms = new MemoryStream())
                {
                    diffImage.Value.Encode(ms, SKEncodedImageFormat.Png, 80);
                    Debug.WriteLine($"Diff image size: {ms.ToArray().Length}");
                }
                Debug.WriteLine($"Diff Image time: {sw.Elapsed.TotalMilliseconds}");


                //sw.Restart();

                //diffSize = 0;

                //using (var tempImage = (Bitmap)frame1.Clone(new Rectangle(diff.X, diff.Y, diff.Width, diff.Height), PixelFormat.Format32bppArgb))
                //{
                //    imageBytes = ImageUtils.EncodeWithSkia(tempImage, SkiaSharp.SKEncodedImageFormat.Webp, 60);
                //    diffSize = imageBytes.Length;
                //}
                //Debug.WriteLine($"WEBP diff size: {diffSize}");
                //Debug.WriteLine($"WEBP diff time: {sw.Elapsed.TotalMilliseconds}");


                //sw.Restart();
                //diffImage = ImageUtils.GetImageDiff(frame1, frame2, false, out hadChanges);

                //imageBytes = ImageUtils.EncodeWithSkia(diffImage, SkiaSharp.SKEncodedImageFormat.Webp, 60);
                //Debug.WriteLine($"WEBP image size: {imageBytes.Length}");
                //Debug.WriteLine($"WEBP Image time: {sw.Elapsed.TotalMilliseconds}");


                Debug.WriteLine($"\n");
            }
        }

        [TestInitialize]
        public void Init()
        {
            _conductor = new Conductor();
            _cursorWatcher = new Mock<ICursorIconWatcher>();
            _sessionIndicator = new Mock<ISessionIndicator>();
            _clipboard = new Mock<IClipboardService>();
            _casterSocket = new Mock<ICasterSocket>();
            _audio = new Mock<IAudioCapturer>();
            _shutdown = new Mock<IShutdownService>();
            _webrtcFactory = new Mock<IWebRtcSessionFactory>();
            _capturer = new ScreenCapturerWin();
            _screenCaster = new ScreenCaster(_conductor, _cursorWatcher.Object, _sessionIndicator.Object, _shutdown.Object);
            _viewer = new Viewer(_casterSocket.Object, _capturer, _clipboard.Object, _webrtcFactory.Object, _audio.Object);

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

        private SKBitmap GetFrame(string frameFileName)
        {
            using var mrs = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Remotely.Desktop.Win.Tests.Resources.{frameFileName}.jpg");
            var resourceImage = (Bitmap)Bitmap.FromStream(mrs);

            if (resourceImage.PixelFormat != PixelFormat.Format32bppArgb)
            {
                return resourceImage
                    .Clone(new Rectangle(0, 0, resourceImage.Width, resourceImage.Height), PixelFormat.Format32bppArgb)
                    .ToSKBitmap();
            }
            return resourceImage.ToSKBitmap();
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();

            return codecs.FirstOrDefault(x => x.FormatID == format.Guid);
        }
    }
}
