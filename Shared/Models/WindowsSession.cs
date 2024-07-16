using System.Runtime.Serialization;

namespace Immense.RemoteControl.Shared.Models;

[DataContract]
public enum WindowsSessionType
{
    Console = 1,
    RDP = 2
}

[DataContract]
public class WindowsSession
{
    [DataMember(Name = "ID")]
    public uint Id { get; set; }
    [DataMember(Name = "Name")]
    public string Name { get; set; } = string.Empty;
    [DataMember(Name = "Type")]
    public WindowsSessionType Type { get; set; }
    [DataMember(Name = "Username")]
    public string Username { get; set; } = string.Empty;
}
