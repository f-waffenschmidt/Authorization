using Fwaqo.Authorization.Core.Models;

namespace Fwaqo.Authorization.Core.Options
{
    public class CacheOptions
    {
        public bool Enabled { get; set; } = true;
        public int ExpiresIn { get; set; } = AuthorizationConstants.General.DefaultCacheLifetime;
    }
}