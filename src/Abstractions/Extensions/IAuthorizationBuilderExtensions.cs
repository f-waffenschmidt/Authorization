using Microsoft.Extensions.DependencyInjection.Extensions;
using Waffenschmidt.AuthZ.Abstractions.Interfaces;

namespace Waffenschmidt.AuthZ.Abstractions.Extensions
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