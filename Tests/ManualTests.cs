using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Remotely.Desktop.Core;
using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Services;
using Remotely.Desktop.Win.Services;
using Remotely.Shared.Models;
using Remotely.Shared.Models.RemoteControlDtos;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        private Viewer _viewer;
        private Mock<IWebRtcSessionFactory> _webrtcFactory;


        [TestMethod]
#if !DEBUG
        [Ignore("Manual test.")]
#endif
        public async Task CaptureTest()
        {
            var request = new ScreenCastRequest()
            {
                NotifyUser = true,
                RequesterName = "Batman",
                ViewerID = "asdf"
            };

            var timeout = Debugger.IsAttached ?
                20_000 :
                5_000;

            _ = Task.Run(async () => await _screenCaster.BeginScreenCasting(request));

            await Task.Delay(timeout);

            _viewer.DisconnectRequested = true;
            var fps = _frameCount / (timeout / 1000);
            Debug.WriteLine($"FPS: {fps}");
            Debug.WriteLine($"KB Sent: {Math.Round(_bytesSent / 1024, 2)}");
            Assert.IsTrue(fps > 0);
            _viewer.Dispose();
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
    }
}
