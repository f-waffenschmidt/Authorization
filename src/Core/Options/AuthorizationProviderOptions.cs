﻿namespace Fwaqo.Authorization.Core.Options
{
    public class AuthorizationProviderOptions
    {
        /// <summary>
        /// Address of authorization provider
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Cache options
        /// </summary>
        public CacheOptions Cache { get; set; } = new();
    }
}