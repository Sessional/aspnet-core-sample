using System.Net;
using System.Reflection;
using Dapper;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using LonelyVale.Api.Users;
using LonelyVale.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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
        _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(o =>
                    {
                        o.AddFilter("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogLevel.None);
                        o.AddFilter("LonelyVale.Api.Exceptions.ExceptionHandler", LogLevel.None);
                        o.AddFilter("Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware", LogLevel.None);
                        o.AddFilter("TestContainers", LogLevel.None);
                    }
                );
                builder.ConfigureTestServices(services =>
                {
                    services.Configure<DatabaseConfiguration>(opts =>
                        opts.ConnectionStrings = new Dictionary<string, string>()
                        {
                            {
                                "Primary",
                                databaseFixture.PostgresContainer.GetConnectionString()
                            }
                        }
                    );
                });
            }
        );
    }

    private class ErrorResponseBody
    {
        public string message { get; set; } = string.Empty;
    }

    [Fact]
    public async Task GetUsersWithoutUserIdFailsBadRequest()
    {
        // https://stebet.net/mocking-jwt-tokens-in-asp-net-core-integration-tests/
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/users");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ErrorResponseBody>();
        Assert.Equal("Unable to determine a user to get. Please supply either a userId or an auth0UserId",
            body.message
        );
    }

    [Fact]
    public async Task GetUsersWithUserIdFailsBadRequest()
    {
        // https://stebet.net/mocking-jwt-tokens-in-asp-net-core-integration-tests/
        var client = _factory.CreateClient();

        var database = _factory.Services.GetService<DatabaseContext>();
        var repository = _factory.Services.GetService<UserRepository>();
        await repository.CreateUser(new UserEntity()
        {
            Id = 1,
            Auth0Id = "auth0|1234"
        });
        var response = await client.GetAsync("/users?id=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GetUserResponse>();
        Assert.Equal(1, body.Id);
    }
}