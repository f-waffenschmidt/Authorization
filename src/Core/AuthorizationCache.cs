using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace Florez4Code.Authorization.Core
{
    public class AuthorizationCache : ApplicationCache<ClaimsIdentity>
    {
        public AuthorizationCache(IMemoryCache cache) : base(cache)
        {
        }

        protected override string GetKey(string key)
        {
            return "Authz" + KeySeparator + key;
        }

    }
}