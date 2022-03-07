
using Microsoft.Extensions.DependencyInjection;

namespace Florez4Code.Authorization.Core.Extensions
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