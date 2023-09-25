using System.Net;
using LonelyVale.Api.Exceptions;
using Microsoft.Extensions.Options;

namespace LonelyVale.Api.Auth0;

public class Auth0ManagementHttpClient
{
    private HttpClient Client { get; }
    private IOptions<Auth0Configuration> Configuration { get; }

    public Auth0ManagementHttpClient(HttpClient client, IOptions<Auth0Configuration> configuration)
    {
        Client = client;
        Configuration = configuration;
    }


    public async Task<Dictionary<string, object>?> GetUserInfoOrNull(string userId)
    {
        var response = await Client.GetAsync(
            $"{Configuration.Value.Tenant}/api/v2/users/{userId}");

        if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        if (response.StatusCode == HttpStatusCode.NotFound) return null;

        throw new HttpClientException("Unable to get user info for that user. ",
            response.StatusCode,
            await response.Content.ReadAsStringAsync());
    }
}