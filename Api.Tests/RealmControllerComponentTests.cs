using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using Dapper;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using LonelyVale.Api.Realms;
using LonelyVale.Api.Users;
using LonelyVale.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Testcontainers.PostgreSql;
using Xunit;

namespace LonelyVale.Api.Tests;

public class RealmControllerComponentTests :
    IClassFixture<WebApplicationFactory<Program>>,
    IClassFixture<DatabaseFixture>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RealmControllerComponentTests(WebApplicationFactory<Program> factory, DatabaseFixture databaseFixture)
    {
        _factory = factory.ConfigureStandardTest(databaseFixture);
    }

    [Fact]
    public async Task GetRealmsReturnsARealm()
    {
        var userAuth0Id = "auth0|12345";
        var client = _factory.CreateClient();
        using var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, $"/realms");
        var sub = new Claim("sub", userAuth0Id);
        var claim = new Claim("scope", RealmScopes.ListRealms);

        var repository = _factory.Services.GetService<UserRepository>();
        Assert.NotNull(repository);
        var user = EntityGenerator.CreateUser() with
        {
            Auth0Id = userAuth0Id
        };
        var userId = await repository.CreateUser(user);

        var realmRepository = _factory.Services.GetService<RealmRepository>();
        Assert.NotNull(realmRepository);
        var realmId = await realmRepository.CreateRealm(EntityGenerator.CreateRealm());

        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer",
                MockJwtTokens.GenerateJwtToken(new List<Claim> { sub, claim }));
        var response = await client.SendAsync(requestMessage);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ListRealmsResponse>();
        Assert.NotNull(body);
        Assert.Equal(1, body.Realms.Count());
        var realm = body.Realms.Single();
        Assert.Equal(realmId, realm.Id);
    }

    [Fact]
    public async Task GetRealmsWithoutTokenFails()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/realms");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}