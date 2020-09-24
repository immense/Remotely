namespace Remotely.Desktop.Core.Services
{
    public interface IWebRtcSessionFactory
    {
        WebRtcSession GetNewSession(Services.Viewer viewer);
    }

    public class WebRtcSessionFactory : IWebRtcSessionFactory
    {
        public WebRtcSessionFactory(IDtoMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
        }
        private IDtoMessageHandler MessageHandler { get; }

        public WebRtcSession GetNewSession(Services.Viewer viewer)
        {
            return new WebRtcSession(viewer, MessageHandler);
        }
    }
}
