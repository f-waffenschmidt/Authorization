using Florez4Code.Authorization.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Florez4Code.Authorization.Core.Extensions
{
    public static class AuthorizationBuilderExtensions
    {
        public static IAuthorizationBuilder AddAuthorizationBuilder<T>(this IAuthorizationBuilder authorizationBuilder)
            where T : class, IAuthorizationProvider
        {
            authorizationBuilder.Services.AddTransient<IAuthorizationProvider, T>();
            return authorizationBuilder;
        }
    }
}