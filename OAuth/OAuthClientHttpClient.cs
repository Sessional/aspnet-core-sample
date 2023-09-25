using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using LonelyVale.OAuth.Caching;
using LonelyVale.OAuth.Configuration;
using LonelyVale.OAuth.Exceptions;
using Microsoft.Extensions.Options;

namespace LonelyVale.OAuth;

public class OAuthClientHttpClient
{
    private HttpClient Client { get; }
    private OAuthClientTokenStorage TokenStorage { get; }
    private IOptions<OAuthClientConfiguration> Configuration { get; }

    public OAuthClientHttpClient(HttpClient client, OAuthClientTokenStorage tokenStorage, IOptions<OAuthClientConfiguration> configuration)
    {
        this.Client = client;
        this.TokenStorage = tokenStorage;
        this.Configuration = configuration;
    }
    
    public async Task<string> GetToken(string oauthClientName)
    {
        return await this.TokenStorage.GetOrUpdate(oauthClientName, GetTokenCallback);
    }
    
    async Task<OAuthClientTokenStorageResult> GetTokenCallback(string name)
    {
        var configuration = this.Configuration.Value.GetClientCredentialsByName(name);
        var response = await this.Client.PostAsJsonAsync(configuration.TokenEndpoint,
        new {
            client_id=configuration.ClientId,
            client_secret=configuration.ClientSecret,
            audience=configuration.Audience,
            grant_type=configuration.GrantType
        });

        if (!response.IsSuccessStatusCode)
        {
            throw new OAuthHttpException(
                "Unable to get the access token via client credentials and OAuth:",
                await response.Content.ReadAsStringAsync()
            );
        }
        
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        if (body == null)
        {
            throw new OAuthException("No response from token endpoint.");
        }

        JsonElement accessTokenAsJsonElement = body["access_token"] as JsonElement? ??
                                       throw new OAuthException("Unable to process the response body to find an access token.");
        string token = accessTokenAsJsonElement.GetString() ?? 
                       throw new OAuthException("Unable to resolve the access token as a string");

        var expiresInValue = body["expires_in"] as JsonElement? ??
                             throw new OAuthException("Unable to process the response body to find an expiration.");
        
        int expirationSeconds = expiresInValue.GetInt32();
        return new OAuthClientTokenStorageResult()
        {
            Value = token,
            Expiration = TimeSpan.FromSeconds(expirationSeconds)
        };
    }

}