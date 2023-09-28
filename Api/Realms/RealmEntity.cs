namespace LonelyVale.Api.Realms;

public class RealmEntity
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string Auth0OrgId { get; set; }

    public bool IsPublic { get; set; }
}