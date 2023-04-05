
using Microsoft.Extensions.DependencyInjection;

namespace Fwaqo.Authorization.Abstractions.Interfaces
{
    public interface IAuthorizationBuilder
    {

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        IServiceCollection Services { get; }

    }
}