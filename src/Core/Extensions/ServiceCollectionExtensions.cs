using System;
using System.Net.Http;
using AuthZ.Abstractions.Extensions;
using AuthZ.Core.Models;
using AuthZ.Core.Options;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Extensions.Http;

namespace AuthZ.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IAuthorizationBuilder AddAuthorizationServices<T>(this IServiceCollection services, Action<AuthorizationProviderOptions> configureOptions) where T : DelegatingHandler
        {
            var authorizationBuilder = new AuthorizationBuilder(services);
            services.TryAddSingleton<IAuthorizationBuilder>(authorizationBuilder);
            authorizationBuilder.AddAuthorizationBuilder<AuthorizationProvider>();
            
            services.AddHttpClient("AuthorizationInvoker")
                .AddHttpMessageHandler<T>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.BadGateway)
                    .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                        retryAttempt))));

            services.AddAuthorization();
            services.AddMemoryCache();
            services.AddTransient<AuthorizationCache>();
            services.Configure(configureOptions);
            services.TryAddTransient<IPolicyEvaluator, CustomPolicyEvaluator>();
            services.AddTransient<IPolicyEvaluator, CustomPolicyEvaluator>();
            return authorizationBuilder;
        }

        public static IAuthorizationBuilder AddAuthorizationServices<T>(this IServiceCollection services) where T : DelegatingHandler
            => services.AddAuthorizationServices<T>(options => { });


        public static IAuthorizationBuilder AddAuthorizationServices<T>(this IServiceCollection services, IConfiguration configuration) where T : DelegatingHandler
            => services.AddAuthorizationServices<T>(options =>
                configuration.GetSection(AuthorizationConstants.General.Authorization).Bind(options));

        
    }
}