﻿using System;
using Fwaqo.Authorization.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Fwaqo.Authorization.Core
{
    /// <summary>
    /// AuthorizationBuilder class to add services.
    /// </summary>
    public sealed class AuthorizationBuilder : IAuthorizationBuilder
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="services">The services being configured.</param>
        public AuthorizationBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// The services being configured.
        /// </summary>
        public IServiceCollection Services { get; }


    }
}