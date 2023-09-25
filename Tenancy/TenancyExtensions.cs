using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LonelyVale.Tenancy;

public static class TenancyExtensions
{
    public static IServiceCollection AddTenancyComponents(this IServiceCollection services)
    {
        services.AddOptions<TenantIdResolverConfiguration>()
            .BindConfiguration(TenantIdResolverConfiguration.SECTION_KEY)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<ITenantIdResolver, DefaultingTenantIdResolver>();
        return services;
    }

    public static IApplicationBuilder UseHeaderBasedTenancy(
        this IApplicationBuilder builder)
    {
        builder.Use(async (context, func) =>
        {
            var configuration = context.RequestServices.GetService<IOptions<TenantIdResolverConfiguration>>();

            if (context.Request.Headers.TryGetValue(configuration.Value.TenantIdHeader, out var tenantId))
                context.Items.Add(configuration.Value.TenantIdKey, tenantId.First());
            await func(context);
        });
        return builder;
    }
}