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
        public WebRtcSessionFactory(IDtoMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
        }
        private IDtoMessageHandler MessageHandler { get; }

        public WebRtcSession GetNewSession(Viewer viewer)
        {
            return new WebRtcSession(viewer, MessageHandler);
        }
    }
}
