using Microsoft.Extensions.Caching.Memory;
using Remotely.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public interface IExpiringTokenService
    {
        string GetToken(DateTimeOffset expiration);
        bool TryGetExpiration(string secret, out DateTimeOffset tokenExpiration);
    }

    public class ExpiringTokenService : IExpiringTokenService
    {
        private static readonly MemoryCache _tokenCache = new(new MemoryCacheOptions());

        public string GetToken(DateTimeOffset expiration)
        {
            var secret = RandomGenerator.GenerateString(36);
            _tokenCache.Set(secret, expiration, expiration);
            return secret;
        }

        public bool TryGetExpiration(string secret, out DateTimeOffset tokenExpiration)
        {
            return _tokenCache.TryGetValue(secret, out tokenExpiration);
        }
    }
}
