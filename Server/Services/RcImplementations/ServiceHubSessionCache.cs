using Immense.RemoteControl.Server.Abstractions;
using System.Collections.Concurrent;

namespace Remotely.Server.Services.RcImplementations
{
    public class ServiceHubSessionCache : IServiceHubSessionCache
    {
        public ConcurrentDictionary<string, string> Sessions => throw new System.NotImplementedException();
    }
}
