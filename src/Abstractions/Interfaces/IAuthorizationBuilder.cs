
using Microsoft.Extensions.DependencyInjection;

namespace AuthZ.Core.Extensions
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