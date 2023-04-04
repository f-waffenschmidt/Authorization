using System;
using Microsoft.Extensions.DependencyInjection;

namespace AuthZ.Core.Extensions
{
    /// <summary>
    /// AuthorizationBuilder class to add services.
    /// </summary>
    public class AuthorizationBuilder : IAuthorizationBuilder
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="services">The services being configured.</param>
        /// <param name="configuration">The configuration.</param>
        public AuthorizationBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// The services being configured.
        /// </summary>
        public virtual IServiceCollection Services { get; }


    }
}