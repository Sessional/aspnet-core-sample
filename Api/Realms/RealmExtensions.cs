using LonelyVale.Api.Auth0;
using LonelyVale.Api.Realms;
using Microsoft.AspNetCore.Authorization;

namespace LonelyVale.Api.Realms;

public static class RealmExtensions
{
    public static IServiceCollection AddRealms(this IServiceCollection services)
    {
        services
            .AddSingleton<RealmRepository>();
        return services;
    }

    public static AuthorizationOptions AddRealmAuthorization(this AuthorizationOptions options)
    {
        options.AddPolicy("GetRealms", policyBuilder =>
            policyBuilder.AddRequirements(new ScopeRequirement("realms:GetRealms"))
                .Build());
        options.AddPolicy("DescribeRealm", policyBuilder =>
            policyBuilder.AddRequirements(new ScopeRequirement("realms:DescribeRealm"))
                .Build());
        return options;
    }
}