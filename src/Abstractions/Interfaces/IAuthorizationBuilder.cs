
using Microsoft.Extensions.DependencyInjection;

namespace Waffenschmidt.AuthZ.Abstractions.Interfaces
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