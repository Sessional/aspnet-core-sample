using LonelyVale.OAuth.Caching;
using LonelyVale.OAuth.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace LonelyVale.OAuth.Extensions;

public static class OAuthClientExtensions
{
    public static IServiceCollection AddOAuthClient(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddOptions<OAuthClientConfiguration>()
            .BindConfiguration(OAuthClientConfiguration.SECTION_KEY)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<OAuthClientTokenStorage>();
        services.AddHttpClient<OAuthClientHttpClient>();
        return services;
    }

    public static IHttpClientBuilder AddOAuthCredentialHandler(this IHttpClientBuilder builder,
        string nameOfOAuthCredentials)
    {
        builder.Services.Configure<HttpClientFactoryOptions>(builder.Name, options =>
        {
            options.HttpMessageHandlerBuilderActions.Add(b => b.AdditionalHandlers.Add(
                    new OAuthClientAuthHandler(
                        b.Services.GetService<OAuthClientHttpClient>()!,
                        nameOfOAuthCredentials
                    )
                )
            );
        });

        return builder;
    }
}