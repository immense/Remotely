using Remotely.Server.Models;
using Remotely.Shared.Services;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Remotely.Server.Services;

/// <summary>
/// A cache containing all active remote control sessions.
/// </summary>
public interface IRemoteControlSessionCache
{
    IEnumerable<RemoteControlSession> Sessions { get; }
    RemoteControlSession AddOrUpdate(string sessionId, RemoteControlSession session);
    RemoteControlSession AddOrUpdate(
        string sessionId,
        RemoteControlSession session,
        Func<string, RemoteControlSession, RemoteControlSession> updateFactory);

    RemoteControlSession GetOrAdd(string sessionId, Func<string, RemoteControlSession> valueFactory);
    Task RemoveExpiredSessions();
    bool TryAdd(string sessionId, RemoteControlSession session);

    bool TryGetValue(string sessionId, [NotNullWhen(true)] out RemoteControlSession? session);
    bool TryRemove(string sessionId, [NotNullWhen(true)] out RemoteControlSession? session);
}

internal class RemoteControlSessionCache : IRemoteControlSessionCache
{
    private readonly ConcurrentDictionary<string, RemoteControlSession> _sessions = new();
    // ConcurrentDictionary's AddOrUpdate and GetOrAdd are not atomic operations,
    // so we need to use an outer lock.
    private readonly object _sessionsLock = new();

    private readonly ILogger<RemoteControlSessionCache> _logger;
    private readonly ISystemTime _systemTime;

    public RemoteControlSessionCache(
        ISystemTime systemTime,
        ILogger<RemoteControlSessionCache> logger)
    {
        _systemTime = systemTime;
        _logger = logger;
    }

    public IEnumerable<RemoteControlSession> Sessions => _sessions.Values;
    public RemoteControlSession AddOrUpdate(string sessionId, RemoteControlSession session)
    {
        lock (_sessionsLock)
        {
            return AddOrUpdate(sessionId, session, (k, v) =>
            {
                v.Dispose();
                return session;
            });
        }
    }

    public RemoteControlSession AddOrUpdate(
        string sessionId,
        RemoteControlSession session,
        Func<string, RemoteControlSession, RemoteControlSession> updateFactory)
    {
        lock (_sessionsLock)
        {
            if (_sessions.ContainsKey(sessionId))
            {
                var newValue = updateFactory(sessionId, _sessions[sessionId]);
                _sessions[sessionId] = newValue;
                return newValue;
            }

            _sessions[sessionId] = session;
            return session;
        }
    }

    public RemoteControlSession GetOrAdd(string sessionId, Func<string, RemoteControlSession> valueFactory)
    {
        lock (_sessionsLock)
        {
            return _sessions.GetOrAdd(sessionId, (key) =>
            {
                return valueFactory(key);
            });
        }
    }

    public Task RemoveExpiredSessions()
    {
        lock (_sessionsLock)
        {
            foreach (var session in _sessions)
            {
                if (session.Value.Mode is RemoteControlMode.Unattended or RemoteControlMode.Unknown &&
                    session.Value.ViewerList.Count == 0 &&
                    session.Value.Created < _systemTime.Now.AddMinutes(-1))
                {
                    _logger.LogWarning("Removing expired session: {session}", JsonSerializer.Serialize(session.Value));
                    if (_sessions.TryRemove(session.Key, out var expiredSession))
                    {
                        expiredSession.Dispose();
                    }
                }
            }
        }
        return Task.CompletedTask;
    }

    public bool TryAdd(string sessionId, RemoteControlSession session)
    {
        lock (_sessionsLock)
        {
            return _sessions.TryAdd(sessionId, session);
        }
    }

    public bool TryGetValue(string sessionId, [NotNullWhen(true)] out RemoteControlSession? session)
    {
        lock (_sessionsLock)
        {
            return _sessions.TryGetValue(sessionId, out session);
        }
    }

    public bool TryRemove(string sessionId, [NotNullWhen(true)] out RemoteControlSession? session)
    {
        lock (_sessionsLock)
        {
            if (_sessions.TryRemove(sessionId, out session))
            {
                try
                {
                    session.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing RemoteControlSession ID {id}.", sessionId);
                }

                return true;
            }
        }
        return false;
    }

}
