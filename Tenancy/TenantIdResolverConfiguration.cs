using System.ComponentModel.DataAnnotations;

namespace LonelyVale.Tenancy;

public class TenantIdResolverConfiguration
{
    public const string SECTION_KEY = "Tenancy";
    [Required] public string DefaultTenantId { get; init; } = string.Empty;

    [Required] public string TenantIdKey { get; init; } = "TenantId";
    [Required] public string TenantIdHeader { get; init; } = "X-Tenant-Id";
}