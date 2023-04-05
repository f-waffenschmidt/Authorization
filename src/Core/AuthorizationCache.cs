using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace Fwaqo.Authorization.Core
{
    /// <summary>
    /// The authorization cache
    /// </summary>
    public class AuthorizationCache : CacheBase<ClaimsIdentity>
    {
        /// <summary>
        /// Instantiates the authorization cache
        /// </summary>
        /// <param name="cache"></param>
        public AuthorizationCache(IMemoryCache cache) : base(cache)
        {
        }

        /// <summary>
        /// Gets the cache key for authorization
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override string GetKey(string key)
        {
            return "AuthZ" + KeySeparator + key;
        }

    }
}