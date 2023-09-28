namespace LonelyVale.OAuth.Caching;

public class OAuthClientTokenStorageResult
{
    public string Value { get; set; } = null!;
    public TimeSpan Expiration { get; set; } = TimeSpan.Zero;
}