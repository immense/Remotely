namespace Remotely.Shared.Models
{
    public enum SessionType
    {
        Console,
        RDP
    }
    public class WindowsSession
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public SessionType Type { get; set; }
        public string Username { get; set; }
    }
}
