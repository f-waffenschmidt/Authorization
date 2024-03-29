﻿using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Fwaqo.Authorization.Abstractions.Interfaces
{
    public interface IAuthorizationProvider
    {
        Task RevokeAuthorizationsAsync(Guid id, CancellationToken cancellationToken);
        Task RevokeAuthorizationsAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
        Task<ClaimsIdentity> InvokeAuthorizationsAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
    }
}