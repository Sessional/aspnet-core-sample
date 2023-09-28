using System.ComponentModel.DataAnnotations;

namespace LonelyVale.OAuth.Configuration;

public class OAuthClientConfiguration
{
    public const string SECTION_KEY = "OAuthClients";

    public Dictionary<string, ClientCredentialsConfiguration> ClientCredentials { get; init; } = null!;

    public ClientCredentialsConfiguration GetClientCredentialsByName(string name)
    {
        if (ClientCredentials.TryGetValue(name, out var config))
            return config ?? throw new Exception("The client credential with that name appears to be not configured.");
        throw new Exception("Unable to find a client credentials configuration with that name.");
    }
}

public class ClientCredentialsConfiguration
{
    [Required] public string ClientId { get; init; } = null!;

    [Required] public string ClientSecret { get; init; } = null!;

    [Required] public string Audience { get; init; } = null!;

    [Required] public string GrantType { get; init; } = null!;

    [Required] public string TokenEndpoint { get; init; } = null!;
}