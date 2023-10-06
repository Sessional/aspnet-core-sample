using LonelyVale.Api.Auth0;
using LonelyVale.Api.Realms;
using Microsoft.AspNetCore.Authorization;

namespace LonelyVale.Api.Realms;

public static class RealmScopes
{
    public const string ListRealms = "realms:ListRealms";

    public const string DescribeRealm = "realms:DescribeRealm";
    public const string CreateRealm = "realms:CreateRealm";
    public const string DeleteRealm = "realms:DeleteRealm";
    public const string UpdateRealm = "realms:UpdateRealm";
}

public static class RealmPolicies
{
    public const string ListRealms = "ListRealms";
    public const string DescribeRealm = "DescribeRealm";

    public static AuthorizationOptions AddRealmAuthorization(this AuthorizationOptions options)
    {
        options.AddPolicy(DescribeRealm, policyBuilder =>
            policyBuilder.AddRequirements(new ScopeRequirement(RealmScopes.DescribeRealm))
                .Build());
        options.AddPolicy(ListRealms, policyBuilder =>
            policyBuilder.AddRequirements(new ScopeRequirement(RealmScopes.ListRealms))
                .Build());
        return options;
    }
}

public static class RealmExtensions
{
    public static IServiceCollection AddRealms(this IServiceCollection services)
    {
        services
            .AddSingleton<RealmRepository>();
        return services;
    }
}