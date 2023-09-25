namespace LonelyVale.Api.Realms;

public class RealmMembershipEntity
{
    public long Id { get; set; }
    public string RealmId { get; set; }
    public string UserId { get; set; }
}