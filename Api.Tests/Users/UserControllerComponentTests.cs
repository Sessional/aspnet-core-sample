using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using LonelyVale.Api.Tests.Fixtures;
using LonelyVale.Api.Tests.Utilities;
using LonelyVale.Api.Users;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LonelyVale.Api.Tests.Users;

public class UserControllerComponentTests :
    IClassFixture<WebApplicationFactory<Program>>,
    IClassFixture<DatabaseFixture>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UserControllerComponentTests(WebApplicationFactory<Program> factory, DatabaseFixture databaseFixture)
    {
        _factory = factory.ConfigureStandardTest(databaseFixture);
    }

    private class ErrorResponseBody
    {
        public string message { get; set; } = string.Empty;
    }

    [Fact]
    public async Task GetUsersWithoutUserIdFailsBadRequest()
    {
        var client = _factory.CreateClient();
        using var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, $"/users");
        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer",
                MockJwtTokens.GenerateJwtToken(new List<Claim>()));
        var response = await client.SendAsync(requestMessage);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ErrorResponseBody>();
        Assert.NotNull(body);
        Assert.Contains("Unable to determine a user to get.",
            body.message
        );
    }

    [Fact]
    public async Task GetUsersWithoutTokenFails()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUsersWithUserIdSucceeds()
    {
        var client = _factory.CreateClient();
        var auth0UserId = Arbitrary.ArbitraryAuth0Id();

        var repository = _factory.Services.GetService<UserRepository>();
        Assert.NotNull(repository);
        var user = EntityGenerator.CreateUser() with
        {
            Auth0Id = auth0UserId
        };
        var userId = await repository.CreateUser(user);

        using var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, $"/users?id={userId}");
        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer",
                MockJwtTokens.GenerateJwtToken(new List<Claim>()));
        var response = await client.SendAsync(requestMessage);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GetUserResponse>();
        Assert.NotNull(body);
        Assert.Equal(userId, body.Id);
        Assert.Equal(auth0UserId, body.Auth0Id);
    }
}