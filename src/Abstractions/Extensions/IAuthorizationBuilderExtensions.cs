using Fwaqo.Authorization.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fwaqo.Authorization.Abstractions.Extensions
{
    public static class AuthorizationBuilderExtensions
    {
        public static IAuthorizationBuilder AddAuthorizationBuilder<T>(this IAuthorizationBuilder authorizationBuilder)
            where T : class, IAuthorizationProvider
        {
            authorizationBuilder.Services.TryAddTransient<IAuthorizationProvider, T>();
            return authorizationBuilder;
        }
    }
}