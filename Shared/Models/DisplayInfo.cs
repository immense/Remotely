using System.Drawing;
using System.Numerics;
using System.Runtime.Serialization;

namespace Immense.RemoteControl.Shared.Models;

[DataContract]
public class DisplayInfo
{
    [DataMember]
    public bool IsPrimary { get; set; }
    [DataMember]
    public Vector2 ScreenSize { get; set; }
    [DataMember]
    public Rectangle MonitorArea { get; set; }
    [DataMember]
    public Rectangle WorkArea { get; set; }
    [DataMember]
    public string DeviceName { get; set; } = string.Empty;
    [DataMember]
    public IntPtr Hmon { get; set; }
}