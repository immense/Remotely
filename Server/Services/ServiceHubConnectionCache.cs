using Immense.RemoteControl.Server.Models;
using Remotely.Shared.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Remotely.Server.Services
{
    public interface IServiceHubSessionCache
    {
        void AddOrUpdateByConnectionId(string connectionId, Device device);
        ICollection<Device> GetAllDevices();
        IEnumerable<string> GetConnectionIdsByDeviceIds(IEnumerable<string> deviceIds);
        bool TryGetByDeviceId(string deviceId, out Device device);
        bool TryGetConnectionId(string deviceId, out string serviceConnectionId);
        bool TryRemoveByConnectionId(string connectionId, out Device device);
    }

    public class ServiceHubSessionCache : IServiceHubSessionCache
    {

        private readonly ConcurrentDictionary<string, Device> _connectionIdToDeviceLookup = new();
        private readonly ConcurrentDictionary<string, string> _deviceIdToConnectionIdLookup = new();

        public void AddOrUpdateByConnectionId(string connectionId, Device device)
        {
            _connectionIdToDeviceLookup.AddOrUpdate(connectionId, device, (k, v) => device);
            _deviceIdToConnectionIdLookup.AddOrUpdate(device.ID, connectionId, (k, v) => connectionId);
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

        public bool TryGetByDeviceId(string deviceId, out Device device)
        {
            if (_deviceIdToConnectionIdLookup.TryGetValue(deviceId, out var connectionId) &&
                _connectionIdToDeviceLookup.TryGetValue(connectionId, out device))
            {
                return true;
            }
            device = Device.Empty;
            return false;
        }

        public bool TryGetConnectionId(string deviceId, out string serviceConnectionId)
        {
            return _deviceIdToConnectionIdLookup.TryGetValue(deviceId, out serviceConnectionId);
        }

        public bool TryRemoveByConnectionId(string connectionId, out Device device)
        {
            if (_connectionIdToDeviceLookup.TryRemove(connectionId, out var lookupResult))
            {
                device = lookupResult;
                _ = _deviceIdToConnectionIdLookup.TryRemove(lookupResult.ID, out _);
                return true;
            }

            device = Device.Empty;
            return false;
        }
    }
}
