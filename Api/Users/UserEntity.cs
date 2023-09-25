namespace LonelyVale.Api.Users;

public class UserEntity
{
    public long Id { get; set; }
    public string Auth0Id { get; set; } = string.Empty;
}