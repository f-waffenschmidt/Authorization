using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Waffenschmidt.AuthZ.Abstractions.Interfaces;
using Waffenschmidt.AuthZ.Core.Extensions;
using Waffenschmidt.AuthZ.Core.Models;
using Waffenschmidt.AuthZ.Core.Options;

namespace Waffenschmidt.AuthZ.Core
{
    /// <summary>
    /// Default authorization provider class to communicate with decoupled system
    /// </summary>
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
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _authorizationCache = authorizationCache ?? throw new ArgumentNullException(nameof(authorizationCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = httpClientFactory.CreateClient("AuthorizationInvoker");
        }

        /// <summary>
        /// Revokes the authorization from cache
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task RevokeAuthorizationsAsync(Guid id, CancellationToken cancellationToken)
        {
            return _authorizationCache.RemoveAsync(id.ToString());
        }

        /// <summary>
        /// Revokes the authorization from cache
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="cancellationToken"></param>
        public async Task RevokeAuthorizationsAsync(ClaimsPrincipal principal,
            CancellationToken cancellationToken = default)
        {
            var key = principal.FindFirstValue(JwtClaimTypes.Subject) ??
                      principal.FindFirstValue(JwtClaimTypes.ClientId);
            await _authorizationCache.RemoveAsync(key);
        }

        /// <summary>
        /// Invokes the configured authorization provider
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> InvokeAuthorizationsAsync(ClaimsPrincipal principal,
            CancellationToken cancellationToken = default)
        {
            var isSubject = principal.HasClaim(p => p.Type == JwtClaimTypes.Subject);
            var key = principal.FindFirstValue(JwtClaimTypes.Subject) ??
                      principal.FindFirstValue(JwtClaimTypes.ClientId);
            var address = _options.Value.Address.TrimEnd('/') +
                          $"/Authorization/Resolve/{(isSubject ? "User" : "Client")}/{key}";

            PrincipalAuthorizations principalAuthorizations;
            ClaimsIdentity authorizationIdentity;
            if (!_options.Value.Cache.Enabled)
            {
                principalAuthorizations = await ProceedAuthorizationInvokeAsync(
                    new PrincipalAuthorizationsRequest(address), cancellationToken);

                authorizationIdentity = GetClaimsIdentity(principalAuthorizations, principal);
                return authorizationIdentity;
            }

            //Cache is enabled so we can try to query data from cache
            var cachedClaimsIdentity = await _authorizationCache.GetAsync(key);
            if (cachedClaimsIdentity != null)
            {
                return cachedClaimsIdentity;
            }

            principalAuthorizations = await ProceedAuthorizationInvokeAsync(
                new PrincipalAuthorizationsRequest(address), cancellationToken);
            authorizationIdentity = GetClaimsIdentity(principalAuthorizations, principal);
            if (authorizationIdentity != null)
            {
                await _authorizationCache.SetAsync(key, authorizationIdentity,
                    TimeSpan.FromSeconds(GetExpirationTime(principalAuthorizations.ExpiresIn)));
            }

            return authorizationIdentity;
        }
        

        private async Task<PrincipalAuthorizations> ProceedAuthorizationInvokeAsync(
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

                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Resolved authorizations {@PrincipalAuthorizations}", authorizations);

                return authorizations;
            }


            _logger.LogError("Resolving authorizations failed with: {ErrorDescription}",
                authorizationResponse.ErrorDescription);
            return null;
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