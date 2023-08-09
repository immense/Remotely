using System;
using System.Collections.Concurrent;

namespace Remotely.Shared.ViewModels;

public class ChatSession
{
    public ConcurrentQueue<ChatHistoryItem> ChatHistory { get; } = new();
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string ExpandedClass => IsExpanded ? "expanded" : "";
    public bool IsExpanded { get; set; }

    public int MissedChats { get; set; }
    public string SessionId { get; } = Guid.NewGuid().ToString();
}
