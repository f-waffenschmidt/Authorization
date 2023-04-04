using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace AuthZ.Core
{
    public class AuthorizationCache : CacheBase<ClaimsIdentity>
    {
        public AuthorizationCache(IMemoryCache cache) : base(cache)
        {
        }

        protected override string GetKey(string key)
        {
            return "AuthZ" + KeySeparator + key;
        }

    }
}