using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Florez4Code.Authorization.Abstractions.Interfaces;
using Florez4Code.Authorization.Core.Extensions;
using Florez4Code.Authorization.Core.Models;
using Florez4Code.Authorization.Core.Options;
using IdentityModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Florez4Code.Authorization.Core
{
    public class AuthorizationProvider : IAuthorizationProvider
    {
        private readonly IOptions<AuthorizationProviderOptions> _options;
        private readonly AuthorizationCache _authorizationCache;
        private readonly ILogger<AuthorizationProvider> _logger;
        private readonly HttpClient _client;

        public AuthorizationProvider(IHttpClientFactory httpClientFactory,
            IOptions<AuthorizationProviderOptions> options, AuthorizationCache authorizationCache,
            ILogger<AuthorizationProvider> logger)
        {
            _options = options;
            _authorizationCache = authorizationCache;
            _logger = logger;
            _client = httpClientFactory.CreateClient("AuthorizationInvoker");
        }

        public async Task RevokeAuthorizationsAsync(Guid id, CancellationToken cancellationToken)
        {
            await _authorizationCache.RemoveAsync(id.ToString());
        }

        public async Task RevokeAuthorizationsAsync(ClaimsPrincipal principal,
            CancellationToken cancellationToken = default)
        {
            var key = principal.FindFirstValue(JwtClaimTypes.Subject) ??
                      principal.FindFirstValue(JwtClaimTypes.ClientId);
            await _authorizationCache.RemoveAsync(key);
        }

        public async Task<ClaimsIdentity> InvokeAuthorizationsAsync(ClaimsPrincipal principal,
            CancellationToken cancellationToken = default)
        {
            var isSubject = principal.HasClaim(p => p.Type == JwtClaimTypes.Subject);
            var key = principal.FindFirstValue(JwtClaimTypes.Subject) ??
                      principal.FindFirstValue(JwtClaimTypes.ClientId);
            var address = _options.Value.Address.TrimEnd('/') +
                          $"/Authorization/Resolve/{(isSubject ? "User" : "Client")}/{key}";

            if (!_options.Value.Cache.Enabled)
            {
                var (identity, authorizations) = await ProceedAuthorizationInvokeAsync(principal,
                    new PrincipalAuthorizationsRequest(address), cancellationToken);
                return identity;
            }

            var cachedClaimsIdentity = await _authorizationCache.GetAsync(key);
            if (cachedClaimsIdentity != null)
            {
                return cachedClaimsIdentity;
            }

            var (claimsIdentity, authz) = await ProceedAuthorizationInvokeAsync(principal,
                new PrincipalAuthorizationsRequest(address), cancellationToken);
            if (claimsIdentity != null)
            {
                await _authorizationCache.SetAsync(key, claimsIdentity,
                    TimeSpan.FromSeconds(GetExpirationTime(authz.ExpiresIn)));
            }

            return claimsIdentity;
        }

        private async Task<(ClaimsIdentity, PrincipalAuthorizations)> ProceedAuthorizationInvokeAsync(
            ClaimsPrincipal principal,
            PrincipalAuthorizationsRequest request, CancellationToken cancellationToken = default)
        {
            var authorizationResponse = await _client.RequestAuthorizationsAsync(request, cancellationToken);
            if (!authorizationResponse.IsError)
            {
                var authorizations = new PrincipalAuthorizations()
                {
                    ExpiresIn = authorizationResponse.ExpiresIn,
                    Permissions = authorizationResponse.Permissions,
                    Roles = authorizationResponse.Roles
                };
                _logger.LogDebug("Resolved authorizations {@PrincipalAuthorizations}", authorizations);

                var claimsIdentity = GetClaimsIdentity(authorizations, principal);
                return (claimsIdentity, authorizations);
            }


            _logger.LogError("Resolving authorizations failed with: {ErrorDescription}",
                authorizationResponse.ErrorDescription);
            return (null, null);
        }

        private int GetExpirationTime(int givenExpiration)
        {
            if (givenExpiration == 0)
            {
                return givenExpiration;
            }

            return givenExpiration > _options.Value.Cache.ExpiresIn ? _options.Value.Cache.ExpiresIn : givenExpiration;
        }


        private static ClaimsIdentity GetClaimsIdentity(PrincipalAuthorizations principalAuthorizations,
            ClaimsPrincipal principal)
        {
            var claims = new List<Claim>();
            if (principal != null && principal.HasClaim(p => p.Type == JwtClaimTypes.Subject))
            {
                var subject = principal.FindFirstValue(JwtClaimTypes.Subject);
                claims.Add(new Claim(JwtClaimTypes.Subject, subject));
                claims.AddRange(principalAuthorizations.Permissions
                    .Select(p => new Claim(AuthorizationConstants.Claims.Role, p)).ToList());
            }
            else
            {
                var clientId = principal.FindFirstValue(JwtClaimTypes.ClientId);
                claims.Add(new Claim(JwtClaimTypes.ClientId, clientId));
            }

            claims.Add(new Claim(JwtClaimTypes.Expiration, principalAuthorizations.ExpiresIn.ToString()));
            claims.AddRange(principalAuthorizations.Permissions
                .Select(p => new Claim(AuthorizationConstants.Claims.Permission, p)).ToList());

            return new ClaimsIdentity(claims);
        }
    }
}