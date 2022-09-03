using Remotely.Shared.Models;
using System.Collections.Concurrent;

namespace Remotely.Server.Services
{
    public interface IServiceHubSessionCache
    {
        /// <summary>
        /// Key is the SignalR connection ID.
        /// </summary>
        ConcurrentDictionary<string, Device> Sessions { get; }
    }

    public class ServiceHubSessionCache : IServiceHubSessionCache
    {
        public ConcurrentDictionary<string, Device> Sessions { get; } = new();
    }
}
