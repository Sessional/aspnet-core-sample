using LonelyVale.OAuth.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LonelyVale.OAuth.Caching;

public class OAuthClientTokenStorage
{
    private IMemoryCache Storage { get; }
    private Dictionary<String, SemaphoreSlim> TokenThrottling { get; } = new();

    public OAuthClientTokenStorage(IMemoryCache cache, IOptions<OAuthClientConfiguration> configuration)
    {
        this.Storage = cache;

        foreach (var clientName in configuration.Value.ClientCredentials.Keys)
        {
            this.TokenThrottling.Add(clientName, new SemaphoreSlim(1, 1));   
        }
    }

    public async Task<string> GetOrUpdate(string name, Func<string, Task<OAuthClientTokenStorageResult>> callback)
    {
        if (Storage.TryGetValue(name, out string? tokenValue))
        {
            if (tokenValue != null)
            {
                return tokenValue;                
            }
        }
        
        await this.TokenThrottling[name].WaitAsync();
        try
        {
            if (Storage.TryGetValue(name, out tokenValue))
            {
                if (tokenValue != null)
                {
                    return tokenValue;                
                }
            }


            OAuthClientTokenStorageResult token = await callback(name);
            this.Storage.Set(name, token.Value, token.Expiration);
            return token.Value;
        }
        finally {
            this.TokenThrottling[name].Release();
        }
    }
}