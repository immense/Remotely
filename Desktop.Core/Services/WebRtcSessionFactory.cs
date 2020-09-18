using Remotely.Desktop.Core.Interfaces;
using Remotely.Desktop.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.Desktop.Core.Services
{
    public interface IWebRtcSessionFactory
    {
        WebRtcSession GetNewSession(Viewer viewer);
    }

    public class WebRtcSessionFactory : IWebRtcSessionFactory
    {
        public WebRtcSessionFactory(CasterSocket casterSocket,
            IDtoMessageHandler messageHandler,
            IKeyboardMouseInput keyboardMouseInput,
            IAudioCapturer audioCapturer,
            IClipboardService clipboardService,
            IFileTransferService fileDownloadService)
        {
            MessageHandler = messageHandler;
            CasterSocket = casterSocket;
            KeyboardMouseInput = keyboardMouseInput;
            AudioCapturer = audioCapturer;
            ClipboardService = clipboardService;
            FileDownloadService = fileDownloadService;
        }
        private IAudioCapturer AudioCapturer { get; }
        private IDtoMessageHandler MessageHandler { get; }
        private CasterSocket CasterSocket { get; }

        private IClipboardService ClipboardService { get; }

        private IFileTransferService FileDownloadService { get; }

        private IKeyboardMouseInput KeyboardMouseInput { get; }

        public WebRtcSession GetNewSession(Viewer viewer)
        {
            return new WebRtcSession(viewer, MessageHandler);
        }
    }
}
