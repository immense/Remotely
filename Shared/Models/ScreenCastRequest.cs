namespace Remotely.Shared.Models
{
    public class ScreenCastRequest
    {
        public bool NotifyUser { get; set; }
        public string RequesterName { get; set; }
        public string ViewerID { get; set; }
        public bool UseWebRtc { get; set; }
    }
}
