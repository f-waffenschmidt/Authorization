using System.Security.Claims;
using System.Threading.Tasks;
using Florez4Code.Authorization.Abstractions.Interfaces;
using Florez4Code.Authorization.Core.Options;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Florez4Code.Authorization.Core
{
    public class CustomPolicyEvaluator : PolicyEvaluator
    {
        private readonly AuthorizationCache _authorizationCache;
        private readonly IAuthorizationProvider _authorizationProvider;
        private readonly IOptions<AuthorizationProviderOptions> _options;

        public CustomPolicyEvaluator(IAuthorizationService authorization, AuthorizationCache authorizationCache,
            IAuthorizationProvider authorizationProvider, IOptions<AuthorizationProviderOptions> options) :
            base(authorization)
        {
            _authorizationCache = authorizationCache;
            _authorizationProvider = authorizationProvider;
            _options = options;
        }

        /// <inheritdoc />
        public override async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
            AuthenticateResult authenticationResult, HttpContext context,
            object? resource)
        {
            if (!authenticationResult.Succeeded)
                return await base.AuthorizeAsync(policy, authenticationResult, context, resource);
            var principal = authenticationResult.Principal;
            var key = principal.FindFirstValue(JwtClaimTypes.Subject) ??
                      principal.FindFirstValue(JwtClaimTypes.ClientId);

            var claimsIdentity = await _authorizationProvider.InvokeAuthorizationsAsync(principal, default);
            context.User.AddIdentity(claimsIdentity);

            return await base.AuthorizeAsync(policy, authenticationResult, context, resource);
        }
    }
}