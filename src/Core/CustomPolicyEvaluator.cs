﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Waffenschmidt.AuthZ.Abstractions.Interfaces;
using Waffenschmidt.AuthZ.Core.Options;

namespace Waffenschmidt.AuthZ.Core
{
    public class CustomPolicyEvaluator : PolicyEvaluator
    {
        private readonly IAuthorizationProvider _authorizationProvider;

        public CustomPolicyEvaluator(IAuthorizationService authorization,
            IAuthorizationProvider authorizationProvider, IOptions<AuthorizationProviderOptions> options) :
            base(authorization)
        {
            _authorizationProvider = authorizationProvider;
        }

        /// <inheritdoc />
        public override async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
            AuthenticateResult authenticationResult, HttpContext context,
            object? resource)
        {
            if (!authenticationResult.Succeeded)
                return await base.AuthorizeAsync(policy, authenticationResult, context, resource);
            var principal = authenticationResult.Principal;
            var claimsIdentity = await _authorizationProvider.InvokeAuthorizationsAsync(principal, context.RequestAborted);
            context.User.AddIdentity(claimsIdentity);

            return await base.AuthorizeAsync(policy, authenticationResult, context, resource);
        }
    }
}