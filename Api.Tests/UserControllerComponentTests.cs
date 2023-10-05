using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using Dapper;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using LonelyVale.Api.Users;
using LonelyVale.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Testcontainers.PostgreSql;
using Xunit;

namespace LonelyVale.Api.Tests;

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
        Assert.Equal("Unable to determine a user to get. Please supply either a userId or an auth0UserId",
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
        var auth0UserId = "auth0|this-is-an-absurd-name";

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