using Microsoft.Extensions.Logging;
using Remotely.Server.Hubs;
using Remotely.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Services;

public interface ICircuitManager
{
    ICollection<ICircuitConnection> Connections { get; }
    bool TryAddConnection(string id, ICircuitConnection connection);
    bool TryRemoveConnection(string id, [NotNullWhen(true)] out ICircuitConnection? connection);
    bool TryGetConnection(string id, [NotNullWhen(true)] out ICircuitConnection? connection);
}
public class CircuitManager : ICircuitManager
{
    private static readonly ConcurrentDictionary<string, ICircuitConnection> _connections = new();
    private readonly ILogger<CircuitManager> _logger;

    public CircuitManager(ILogger<CircuitManager> logger)
    {
        _logger = logger;
    }

    public ICollection<ICircuitConnection> Connections => _connections.Values;


    public bool TryAddConnection(string id, ICircuitConnection connection)
    {
        return _connections.TryAdd(id, connection);
    }

    public bool TryGetConnection(string id, [NotNullWhen(true)] out ICircuitConnection? connection)
    {
        return _connections.TryGetValue(id, out connection);
    }

    public bool TryRemoveConnection(string id, [NotNullWhen(true)] out ICircuitConnection? connection)
    {
        return _connections.TryRemove(id, out connection);
    }
}
