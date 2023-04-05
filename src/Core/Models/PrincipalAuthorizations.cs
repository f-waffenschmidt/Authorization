using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fwaqo.Authorization.Core.Models
{
    public class PrincipalAuthorizations
    {
        [JsonProperty("roles")]
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        [JsonProperty("permissions")]
        public IEnumerable<string> Permissions { get; set; } = new List<string>();
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; } = 3600;
    }
}