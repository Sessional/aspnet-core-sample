using System.Net.Http.Headers;

namespace LonelyVale.OAuth;

public class OAuthClientAuthHandler : DelegatingHandler
{
    private OAuthClientHttpClient Client { get; }
    private string Name { get; }
    
    public OAuthClientAuthHandler(OAuthClientHttpClient httpClient, string name)
    {
        this.Client = httpClient;
        this.Name = name;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization == null || !request.Headers.Contains("Authorization"))
        {
            var token = await Client.GetToken(Name);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}