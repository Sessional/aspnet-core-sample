namespace LonelyVale.Tenancy;

public interface ITenantIdResolver
{
    public string GetTenantId();
}