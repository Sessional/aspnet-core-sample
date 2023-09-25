using LonelyVale.OAuth.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace LonelyVale.Api.Auth0;

public static class Auth0Extensions
{
    public static IServiceCollection AddAuth0(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        services.AddOAuthClient();
        services.AddOptions<Auth0Configuration>()
            .BindConfiguration(Auth0Configuration.SECTION_KEY)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAuthorizationHandler, ScopeRequirementHandler>();

        services.AddHttpClient<Auth0ManagementHttpClient>()
            .AddOAuthCredentialHandler("Auth0ManagementApi");

        return services;
    }
}