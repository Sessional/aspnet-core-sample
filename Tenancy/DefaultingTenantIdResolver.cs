using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LonelyVale.Tenancy;

public class DefaultingTenantIdResolver : ITenantIdResolver
{
    private IHttpContextAccessor HttpContextAccessor { get; }
    private IOptions<TenantIdResolverConfiguration> Configuration { get; }

    public DefaultingTenantIdResolver(IHttpContextAccessor contextAccessor,
        IOptions<TenantIdResolverConfiguration> configuration)
    {
        HttpContextAccessor = contextAccessor;
        Configuration = configuration;
    }

    public string GetTenantId()
    {
        if (HttpContextAccessor.HttpContext == null) return Configuration.Value.DefaultTenantId;

        var orgId = HttpContextAccessor.HttpContext.User.FindFirstValue("org_id");
        HttpContextAccessor.HttpContext.Items.TryGetValue(Configuration.Value.TenantIdKey, out var tenantId);
        var tenantIdString = tenantId as string;
        return tenantIdString ?? orgId ?? Configuration.Value.DefaultTenantId;
    }
}