namespace LonelyVale.Api.Realms;

public class RealmEntity
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Auth0OrgId { get; set; }

    public bool IsPublic { get; set; }
}