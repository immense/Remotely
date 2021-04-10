using Microsoft.Extensions.Logging;
using Remotely.Server.Hubs;
using Remotely.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public interface ICircuitManager
    {
        ICollection<ICircuitConnection> Connections { get; }
        bool TryAddConnection(string id, ICircuitConnection connection);
        bool TryRemoveConnection(string id, out ICircuitConnection connection);
        Task<bool> InvokeOnConnection(string browserConnectionId, CircuitEventName eventName, params object[] args);
        bool TryGetConnection(string browserConnectionId, out ICircuitConnection connection);
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

        public Task<bool> InvokeOnConnection(string browserConnectionId, CircuitEventName eventName, params object[] args)
        {
            try
            {
                if (_connections.TryGetValue(browserConnectionId, out var result))
                {
                    result.InvokeCircuitEvent(eventName, args);
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while invoking circuit event.");
                return Task.FromResult(false);
            }
        }

        public bool TryAddConnection(string id, ICircuitConnection connection)
        {
            return _connections.TryAdd(id, connection);
        }

        public bool TryGetConnection(string connectionId, out ICircuitConnection connection)
        {
            return _connections.TryGetValue(connectionId, out connection);
        }

        public bool TryRemoveConnection(string id, out ICircuitConnection connection)
        {
            return _connections.TryRemove(id, out connection);
        }
    }
}
