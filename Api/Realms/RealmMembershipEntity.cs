namespace LonelyVale.Api.Realms;

public class RealmMembershipEntity
{
    public long Id { get; set; }
    public required string RealmId { get; set; }
    public required string UserId { get; set; }
}