namespace LonelyVale.OAuth.Caching;

public class OAuthClientTokenStorageResult
{
    public string Value { get; set; }
    public TimeSpan Expiration { get; set; }
}