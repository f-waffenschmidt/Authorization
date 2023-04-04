using AuthZ.Abstractions.Interfaces;
using AuthZ.Core.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthZ.Abstractions.Extensions
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