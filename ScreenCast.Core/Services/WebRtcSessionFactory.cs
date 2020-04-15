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
         IClipboardService clipboardService)
        {
            CasterSocket = casterSocket;
            KeyboardMouseInput = keyboardMouseInput;
            AudioCapturer = audioCapturer;
            ClipboardService = clipboardService;
        }
        public WebRtcSession GetNewSession(Viewer viewer)
        {
            var messageHandler = new RtcMessageHandler(viewer,
                CasterSocket,
                KeyboardMouseInput,
                AudioCapturer,
                ClipboardService);

            return new WebRtcSession(messageHandler);
        }
        private CasterSocket CasterSocket { get; }
        private IKeyboardMouseInput KeyboardMouseInput { get; }
        private IAudioCapturer AudioCapturer { get; }
        private IClipboardService ClipboardService { get; }
    }
}
