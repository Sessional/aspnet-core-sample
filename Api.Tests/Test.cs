using System.Net;
using System.Reflection;
using LonelyVale.Api.Users;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace LonelyVale.Api.Tests;

public class Test : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Test(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public record ErrorResponseBody
    {
        public string message { get; set; }
    }

    [Fact]
    public async Task Test1()
    {
        // https://stebet.net/mocking-jwt-tokens-in-asp-net-core-integration-tests/
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/users");

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        var body = await response.Content.ReadFromJsonAsync<ErrorResponseBody>();
        Assert.Equal("Unable to determine a user to get. Please supply either a userId or an auth0UserId",
            body.message
        );
    }
}