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

public class Test :
    IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private PostgreSqlContainer _postgreSqlContainer;
    private IContainer _atlasGo;

    public Test(WebApplicationFactory<Program> factory)
    {
        TestcontainersSettings.Logger =
            new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider().CreateLogger("TestContainers");
        _postgreSqlContainer = new PostgreSqlBuilder().Build();

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
                                $"{_postgreSqlContainer.GetConnectionString()}"
                            }
                        }
                    );
                });
            }
        );
    }

    private class ErrorResponseBody
    {
        public string message { get; set; }
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
        using var connection = database.GetConnection("Primary");
        await connection
            .ExecuteAsync(
                """
                INSERT INTO users (id, auth0_id) VALUES (1,'auth0|1234')
                """);
        var response = await client.GetAsync("/users?id=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GetUserResponse>();
        Assert.Equal(1, body.Id);
    }

    public static class DirectoryHelper
    {
        public static DirectoryInfo TryGetSolutionDirectoryInfo(string? currentPath = null)
        {
            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any()) directory = directory.Parent;
            return directory!;
        }
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        var connectionString = _postgreSqlContainer.GetConnectionString();
        var parts = connectionString.Split(";");
        var port = parts.First(x => x.StartsWith("Port")).Split("=")[1];
        var database = parts.First(x => x.StartsWith("Database")).Split("=")[1];
        var username = parts.First(x => x.StartsWith("Username")).Split("=")[1];
        var password = parts.First(x => x.StartsWith("Password")).Split("=")[1];
        var atlasConnectionString =
            $"postgres://{username}:{password}@host.docker.internal:{port}/{database}?sslmode=disable";
        var directory = DirectoryHelper.TryGetSolutionDirectoryInfo();
        _atlasGo =
            new ContainerBuilder()
                .WithImage("arigaio/atlas:latest")
                .WithResourceMapping(
                    new DirectoryInfo($"{directory}/Migrations/tenant"),
                    "/migrations")
                .WithCommand(
                    "schema",
                    "apply",
                    "--auto-approve",
                    "--url",
                    atlasConnectionString,
                    "--to",
                    "file://migrations",
                    "--var",
                    "tenant_id=1",
                    "--schema=tenant_1,public")
                .Build();

        await _atlasGo.StartAsync();
        // var output = await _atlasGo.GetLogsAsync();
        //
        //
        // var output2 = await _atlasGo.GetLogsAsync();

        await _atlasGo.GetExitCodeAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
    }
}