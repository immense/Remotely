using Immense.RemoteControl.Server.Abstractions;
using System.Collections.Concurrent;

namespace Remotely.Server.Services.RcImplementations
{
    public class ServiceHubSessionCache : IServiceHubSessionCache
    {
        private static readonly ConcurrentDictionary<string, string> _sessions = new();
        public ConcurrentDictionary<string, string> Sessions => _sessions;
    }
}
