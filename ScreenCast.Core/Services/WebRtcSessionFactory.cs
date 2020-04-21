using Remotely.ScreenCast.Core.Communication;
using Remotely.ScreenCast.Core.Interfaces;
using Remotely.ScreenCast.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.ScreenCast.Core.Services
{
    public interface IWebRtcSessionFactory
    {
        WebRtcSession GetNewSession(Viewer viewer);
    }

    public class WebRtcSessionFactory : IWebRtcSessionFactory
    {
        public WebRtcSessionFactory(CasterSocket casterSocket,
         IKeyboardMouseInput keyboardMouseInput,
         IAudioCapturer audioCapturer,
         IClipboardService clipboardService,
         IFileTransferService fileDownloadService)
        {
            CasterSocket = casterSocket;
            KeyboardMouseInput = keyboardMouseInput;
            AudioCapturer = audioCapturer;
            ClipboardService = clipboardService;
            FileDownloadService = fileDownloadService;
        }
        private IAudioCapturer AudioCapturer { get; }

        private CasterSocket CasterSocket { get; }

        private IClipboardService ClipboardService { get; }

        private IFileTransferService FileDownloadService { get; }

        private IKeyboardMouseInput KeyboardMouseInput { get; }

        public WebRtcSession GetNewSession(Viewer viewer)
        {
            var messageHandler = new RtcMessageHandler(viewer,
                CasterSocket,
                KeyboardMouseInput,
                AudioCapturer,
                ClipboardService,
                FileDownloadService);

            return new WebRtcSession(messageHandler);
        }
    }
}
