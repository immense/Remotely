using Remotely.Shared.ViewModels;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Remotely.Server.Services.Stores;

public interface IChatSessionStore
{
    void AddOrUpdate(
        string deviceId,
        ChatSession chatSession,
        Func<string, ChatSession, ChatSession> updateFactory);

    bool ContainsKey(string deviceId);
    ICollection<ChatSession> GetAllSessions();
    ChatSession GetOrAdd(string deviceId, Func<string, ChatSession> factory);
    bool TryGetSession(string deviceId, [NotNullWhen(true)] out ChatSession? session);
    bool TryRemove(string deviceId, [NotNullWhen(true)] out ChatSession? session);
}

public class ChatSessionStore : IChatSessionStore
{
    private readonly ConcurrentDictionary<string, ChatSession> _sessions = new();

    public void AddOrUpdate(
        string deviceId,
        ChatSession chatSession,
        Func<string, ChatSession, ChatSession> updateFactory)
    {
        _sessions.AddOrUpdate(deviceId, chatSession, updateFactory);
    }

    public bool ContainsKey(string deviceId)
    {
        return _sessions.ContainsKey(deviceId);
    }

    public ICollection<ChatSession> GetAllSessions() => _sessions.Values;

    public ChatSession GetOrAdd(string deviceId, Func<string, ChatSession> factory)
    {
        return _sessions.GetOrAdd(deviceId, factory);
    }

    public bool TryGetSession(string deviceId, [NotNullWhen(true)] out ChatSession? session)
    {
        return _sessions.TryGetValue(deviceId, out session);
    }

    public bool TryRemove(string deviceId, [NotNullWhen(true)] out ChatSession? session)
    {
        return _sessions.TryRemove(deviceId, out session);
    }
}
