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
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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
        private Viewer _viewer;
        private Mock<IWebRtcSessionFactory> _webrtcFactory;


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

            _ = Task.Run(async () => await _screenCaster.BeginScreenCasting(request));

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
                var encoderParams = new EncoderParameters()
                {
                    Param = new EncoderParameter[]
                    {
                        new EncoderParameter(Encoder.Quality, 60L)

                    }
                };

                using var frame1 = GetFrame("Frame1");
                using var frame2 = GetFrame("Frame2");
                var jpegEncoder = GetEncoder(ImageFormat.Jpeg);

                var sw = Stopwatch.StartNew();


                sw.Restart();
                var diff = ImageUtils.GetDiffArea(frame1, frame2, false);

                var diffSize = 0;
                using (var tempImage = (Bitmap)frame1.Clone(new Rectangle(diff.X, diff.Y, diff.Width, diff.Height), PixelFormat.Format32bppArgb))
                {
                    using var ms = new MemoryStream();
                    tempImage.Save(ms, jpegEncoder, encoderParams);
                    diffSize = ms.ToArray().Length;
                }
                Debug.WriteLine($"Diff area size: {diffSize}");
                Debug.WriteLine($"Diff area time: {sw.Elapsed.TotalMilliseconds}");


                sw.Restart();
                var diffImage = ImageUtils.GetImageDiff(frame1, frame2, false, out var hadChanges);
                
                
                using (var ms = new MemoryStream())
                {
                    diffImage.Save(ms, ImageFormat.Png);
                    Debug.WriteLine($"Diff image size: {ms.ToArray().Length}");
                }
                Debug.WriteLine($"Diff Image time: {sw.Elapsed.TotalMilliseconds}");


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
            _webrtcFactory = new Mock<IWebRtcSessionFactory>();
            _casterSocket = new Mock<ICasterSocket>();
            _audio = new Mock<IAudioCapturer>();
            _shutdown = new Mock<IShutdownService>();
            _capturer = new ScreenCapturerWin();
            _screenCaster = new ScreenCaster(_conductor, _cursorWatcher.Object, _sessionIndicator.Object, _shutdown.Object);
            _viewer = new Viewer(_casterSocket.Object, _capturer, _clipboard.Object, _webrtcFactory.Object, _audio.Object);

            _casterSocket
                .Setup(x => x.SendDtoToViewer(It.IsAny<CaptureFrameDto>(), It.IsAny<string>()))
                .Callback((CaptureFrameDto dto, string viewerId) => {
                    _bytesSent += dto.ImageBytes.Length;
                });

            _casterSocket
                .Setup(x => x.SendDtoToViewer(It.Is<CaptureFrameDto>(x => x.EndOfFrame), It.IsAny<string>()))
                .Callback(() => {
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

        private Bitmap GetFrame(string frameFileName)
        {
            using var mrs = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Remotely.Tests.Resources.{frameFileName}.jpg");
            var resourceImage = (Bitmap)Bitmap.FromStream(mrs);

            if (resourceImage.PixelFormat != PixelFormat.Format32bppArgb)
            {
                return resourceImage.Clone(new Rectangle(0, 0, resourceImage.Width, resourceImage.Height), PixelFormat.Format32bppArgb);
            }
            return resourceImage;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();

            return codecs.FirstOrDefault(x => x.FormatID == format.Guid);
        }
    }
}
