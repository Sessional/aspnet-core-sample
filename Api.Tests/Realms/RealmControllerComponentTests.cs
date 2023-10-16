using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using LonelyVale.Api.Realms;
using LonelyVale.Api.Tests.Fixtures;
using LonelyVale.Api.Tests.Utilities;
using LonelyVale.Api.Users;
using LonelyVale.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LonelyVale.Api.Tests.Realms;

public class RealmControllerComponentTests :
    IClassFixture<WebApplicationFactory<Program>>,
    IClassFixture<DatabaseFixture>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly DatabaseFixture _databaseFixture;

    public RealmControllerComponentTests(WebApplicationFactory<Program> factory, DatabaseFixture databaseFixture)
    {
        _factory = factory.ConfigureStandardTest(databaseFixture);
        _databaseFixture = databaseFixture;
    }


    [Fact]
    public async Task GetRealmsWithoutTokenFails()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/realms");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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
    public async Task GetRealmsForASpecificUserReturnsARealm()
    {
        await _factory.Services.GetService<DatabaseContext>().ClearTables("realms");
        var userAuth0Id = Arbitrary.ArbitraryAuth0Id();
        var otherUserAuth0Id = Arbitrary.ArbitraryAuth0Id();
        var client = _factory.CreateClient();
        using var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, $"/realms?auth0UserId={otherUserAuth0Id}");
        var sub = new Claim("sub", userAuth0Id);
        var claim = new Claim("scope", RealmScopes.ListRealms);

        var repository = _factory.Services.GetService<UserRepository>();
        Assert.NotNull(repository);
        var user = EntityGenerator.CreateUser() with
        {
            Auth0Id = userAuth0Id
        };
        var user2 = EntityGenerator.CreateUser() with
        {
            Auth0Id = otherUserAuth0Id
        };
        var userId = await repository.CreateUser(user);
        var user2Id = await repository.CreateUser(user2);

        var realmRepository = _factory.Services.GetService<RealmRepository>();
        Assert.NotNull(realmRepository);
        var realmId = await realmRepository.CreateRealm(EntityGenerator.CreateRealm());

        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer",
                MockJwtTokens.GenerateJwtToken(new List<Claim> { sub, claim }));
        var response = await client.SendAsync(requestMessage);

        var bodyAsString = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ListRealmsResponse>();
        Assert.NotNull(body);
        Assert.Equal(1, body.Realms.Count());
        var realm = body.Realms.Single();
        Assert.Equal(realmId, realm.Id);
    }
}