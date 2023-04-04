using Waffenschmidt.AuthZ.Core.Models;

namespace Waffenschmidt.AuthZ.Core.Options
{
    public class CacheOptions
    {
        public bool Enabled { get; set; } = true;
        public int ExpiresIn { get; set; } = AuthorizationConstants.General.DefaultCacheLifetime;
    }
}