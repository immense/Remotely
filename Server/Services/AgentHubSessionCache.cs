using Immense.RemoteControl.Server.Models;
using Remotely.Shared.Entities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Remotely.Server.Services;

public interface IAgentHubSessionCache
{
    void AddOrUpdateByConnectionId(string connectionId, Device device);
    IEnumerable<string> FilterDevicesByOnlineStatus(IEnumerable<string> deviceIds, bool isOnline);

    ICollection<Device> GetAllDevices();
    IEnumerable<string> GetConnectionIdsByDeviceIds(IEnumerable<string> deviceIds);
    bool TryGetByDeviceId(string deviceId, [NotNullWhen(true)] out Device? device);
    bool TryGetConnectionId(string deviceId, [NotNullWhen(true)] out string? serviceConnectionId);
    bool TryRemoveByConnectionId(string connectionId, [NotNullWhen(true)] out Device? device);
}

public class AgentHubSessionCache : IAgentHubSessionCache
{

    private readonly ConcurrentDictionary<string, Device> _connectionIdToDeviceLookup = new();
    private readonly ConcurrentDictionary<string, string> _deviceIdToConnectionIdLookup = new();

    public void AddOrUpdateByConnectionId(string connectionId, Device device)
    {
        _connectionIdToDeviceLookup.AddOrUpdate(connectionId, device, (k, v) => device);
        _deviceIdToConnectionIdLookup.AddOrUpdate(device.ID, connectionId, (k, v) => connectionId);
    }

    public IEnumerable<string> FilterDevicesByOnlineStatus(IEnumerable<string> deviceIds, bool isOnline)
    {
        foreach (var deviceId in deviceIds)
        {
            var result = TryGetConnectionId(deviceId, out _);
            if (result == isOnline)
            {
                yield return deviceId;
            }
        }
    }

    public ICollection<Device> GetAllDevices() => _connectionIdToDeviceLookup.Values;

    public IEnumerable<string> GetConnectionIdsByDeviceIds(IEnumerable<string> deviceIds)
    {
        foreach (var deviceId in deviceIds)
        {
            if (TryGetConnectionId(deviceId, out var connectionId))
            {
                yield return connectionId;
            }
        }
    }

    public bool TryGetByDeviceId(string deviceId, [NotNullWhen(true)] out Device? device)
    {
        if (_deviceIdToConnectionIdLookup.TryGetValue(deviceId, out var connectionId) &&
            _connectionIdToDeviceLookup.TryGetValue(connectionId, out device))
        {
            return true;
        }
        device = default;
        return false;
    }

    public bool TryGetConnectionId(string deviceId, [NotNullWhen(true)] out string? serviceConnectionId)
    {
        return _deviceIdToConnectionIdLookup.TryGetValue(deviceId, out serviceConnectionId);
    }

    public bool TryRemoveByConnectionId(string connectionId, [NotNullWhen(true)] out Device? device)
    {
        if (_connectionIdToDeviceLookup.TryRemove(connectionId, out var lookupResult))
        {
            device = lookupResult;
            _ = _deviceIdToConnectionIdLookup.TryRemove(lookupResult.ID, out _);
            return true;
        }

        device = null;
        return false;
    }
}
